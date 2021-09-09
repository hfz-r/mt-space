using System.Reflection;
using AHAM.Services.Commission.API.Application.Behaviors;
using AHAM.Services.Commission.API.Application.DomainEvents;
using AHAM.Services.Commission.API.Application.Queries;
using Autofac;
using FluentValidation;
using MediatR;

namespace AHAM.Services.Commission.API.Application.Modules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(GetRebatesQuery).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
            builder.RegisterAssemblyTypes(typeof(RebateCreatedDomainEventHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(INotificationHandler<>));
            builder.RegisterAssemblyTypes(typeof(RebateQueryValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return type => componentContext.Resolve(type);
            });

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}