using System;
using System.Data.Common;
using System.Reflection;
using AHAM.BuildingBlocks.EventBus;
using AHAM.BuildingBlocks.EventBus.Abstractions;
using AHAM.BuildingBlocks.EventBusRabbitMQ;
using AHAM.BuildingBlocks.IntegrationEventLog;
using AHAM.BuildingBlocks.IntegrationEventLog.Services;
using AHAM.Services.Investor.API.Application.IntegrationEvents;
using AHAM.Services.Investor.API.Infrastructure.Filters;
using AHAM.Services.Investor.Infrastructure;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace AHAM.Services.Investor.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpc(options => { options.EnableDetailedErrors = true; });

            return services;
        }

        public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration);
            services.AddApplicationInsightsKubernetesEnricher();

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services
                .AddControllers(options => { options.Filters.Add(typeof(HttpGlobalExceptionFilter)); })
                .AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = services.AddHealthChecks();

            builder.AddCheck("self", () => HealthCheckResult.Healthy());
            builder.AddSqlServer(
                configuration["ConnectionString"],
                name: "investordb-check",
                tags: new[] {"investordb"});
            builder.AddRabbitMQ(
                $"amqp://{configuration["EventBusConnection"]}",
                name: "investor-rabbitmqbus-check",
                tags: new[] {"rabbitmqbus"});

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<InvestorContext>(options =>
                {
                    options.UseSqlServer(configuration["ConnectionString"],
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                        });

                    if (bool.Parse(configuration["UseRowNumberForPaging"]))
                        options
                            .ReplaceService<IQueryTranslationPostprocessorFactory,
                                SqlServer2008QueryTranslationPostprocessorFactory>();
                });

            services
                .AddDbContext<IntegrationEventLogContext>(options =>
                {
                    options.UseSqlServer(configuration["ConnectionString"],
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                            sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                        });
                });

            return services;
        }

        public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("Redis")["ConnectionString"];
            });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AHAM MT-Space - Investor HTTP API",
                    Version = "v1",
                    Description = "The Investor Service HTTP API"
                });

                //options.AddSecurityDefinition TODO
            });

            return services;
        }

        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddTransient<IIdentityService>(); TODO
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(svc => connection => new IntegrationEventLogService(connection));
            services.AddTransient<IInvestorIntegrationEventService, InvestorIntegrationEventService>();
            services.AddSingleton<IRabbitMQPersistentConnection>(svc =>
            {
                var logger = svc.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                var retryCount = 5;
                var factory = new ConnectionFactory
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                    factory.UserName = configuration["EventBusUserName"];
                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                    factory.Password = configuration["EventBusPassword"];
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);

                return new RabbitMQPersistentConnection(factory, logger, retryCount);
            });

            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = {"application/problem+json", "application/problem+xml"}
                    };
                };
            });

            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(svc =>
            {
                var connection = svc.GetRequiredService<IRabbitMQPersistentConnection>();
                var lifetime = svc.GetRequiredService<ILifetimeScope>();
                var logger = svc.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var manager = svc.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);

                return new EventBusRabbitMQ(connection, logger, lifetime, manager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //todo: add custom auth

            return services;
        }
    }
}