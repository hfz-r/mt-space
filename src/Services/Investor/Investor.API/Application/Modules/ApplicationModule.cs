using AHAM.BuildingBlocks.RedisCache;
using AHAM.Services.Investor.Infrastructure;
using AHAM.Services.Investor.Infrastructure.Idempotent;
using AHAM.Services.Investor.Infrastructure.Repositories;
using Autofac;

namespace AHAM.Services.Investor.API.Application.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork<InvestorContext>>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<RequestManager>().As<IRequestManager>().InstancePerLifetimeScope();
            builder.RegisterType<CacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
        }
    }
}