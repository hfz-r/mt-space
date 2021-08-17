using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.EventBus;
using AHAM.BuildingBlocks.EventBus.Abstractions;
using AHAM.BuildingBlocks.EventBus.Events;
using AHAM.BuildingBlocks.EventBus.Extensions;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace AHAM.BuildingBlocks.EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private const string BrokerName = "aham_event_bus";
        private const string AutofacScopeName = "aham_event_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger,
            ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string queueName = null,
            int retryCount = 5)
        {
            _persistentConnection =
                persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _autofac = autofac;
            _retryCount = retryCount;
            _subsManager.OnEventRemoved += OnEventRemoved;
        }

        #region Private methods

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: BrokerName, type: "direct");
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.CallbackException += (sender, args) =>
            {
                _logger.LogWarning(args.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

            using var channel = _persistentConnection.CreateModel();
            channel.QueueUnbind(queue: _queueName, exchange: BrokerName, routingKey: eventName);

            if (_subsManager.IsEmpty)
            {
                _queueName = String.Empty;
                _consumerChannel.Close();
            }
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

                using var channel = _persistentConnection.CreateModel();
                channel.QueueBind(queue: _queueName, exchange: BrokerName, routingKey: eventName);
            }
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += OnConsumerReceived;

                _consumerChannel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            }
            else _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
        }

        private async Task OnConsumerReceived(object sender, BasicDeliverEventArgs args)
        {
            var eventName = args.RoutingKey;
            var message = Encoding.UTF8.GetString(args.Body);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");

                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            _consumerChannel.BasicAck(args.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using var scope = _autofac.BeginLifetimeScope(AutofacScopeName);
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        if (!(scope.ResolveOptional(subscription.HandlerType) is IDynamicIntegrationEventHandler handler)) continue;
                        dynamic eventData = JObject.Parse(message);

                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;

                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task) concreteType.GetMethod("Handle")?.Invoke(handler, new[] {integrationEvent});
                    }
                }
            }
            else _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
        }

        #endregion

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected) _persistentConnection.TryConnect();

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {
                        _logger.LogWarning(ex,
                            "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id,
                            $"{time.TotalSeconds:n1}", ex.Message);
                    });

            var eventName = @event.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            // ReSharper disable AccessToDisposedClosure
            using var channel = _persistentConnection.CreateModel();

            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

            channel.ExchangeDeclare(exchange: BrokerName, type: "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; //persistent

                _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                channel.BasicPublish(exchange: BrokerName, routingKey: eventName, mandatory: true, basicProperties: properties, body: body);
            });
            // ReSharper restore AccessToDisposedClosure
        }

        public void SubscribeDynamic<Th>(string eventName) where Th : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName,
                typeof(Th).GetGenericTypeName());

            DoInternalSubscription(eventName);
            _subsManager.AddDynamicSubscription<Th>(eventName);
            StartBasicConsume();
        }

        public void Subscribe<T, Th>() where T : IntegrationEvent where Th : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName,
                typeof(Th).GetGenericTypeName());

            _subsManager.AddSubscription<T, Th>();
            StartBasicConsume();
        }

        public void Unsubscribe<T, Th>() where T : IntegrationEvent where Th : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, Th>();
        }

        public void UnsubscribeDynamic<Th>(string eventName) where Th : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<Th>(eventName);
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subsManager.Clear();
        }
    }
}