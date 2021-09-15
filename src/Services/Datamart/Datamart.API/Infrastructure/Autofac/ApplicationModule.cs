using System.Reflection;
using System.Data;
using System.Data.SqlClient;
using AHAM.BuildingBlocks.RedisCache;
using AHAM.Services.Datamart.API.Infrastructure.Repositories;
using AHAM.Services.Datamart.API.Infrastructure.Validations;
using Autofac;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace AHAM.Services.Datamart.API.Infrastructure.Autofac
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            builder.RegisterType<CimbRepository>().As<ICimbRepository>().InstancePerLifetimeScope();
            
            // dapper connection
            builder.Register(context =>
                {
                    var configuration = context.Resolve<IConfiguration>();
                    return new SqlConnection(configuration["ConnectionString"]);
                })
                .As<IDbConnection>();

            // validator registration
            builder.RegisterAssemblyTypes(typeof(CimbCommandValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();
        }
    }
}