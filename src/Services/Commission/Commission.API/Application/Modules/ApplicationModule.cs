using AHAM.BuildingBlocks.RedisCache;
using AHAM.Services.Commission.Infrastructure;
using AHAM.Services.Commission.Infrastructure.Idempotent;
using AHAM.Services.Commission.Infrastructure.Repositories;
using Autofac;

namespace AHAM.Services.Commission.API.Application.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork<CommissionContext>>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<RequestManager>().As<IRequestManager>().InstancePerLifetimeScope();
            builder.RegisterType<CacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
        }
    }
}