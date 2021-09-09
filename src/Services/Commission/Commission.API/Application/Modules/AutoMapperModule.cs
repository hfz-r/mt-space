using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using Module = Autofac.Module;

namespace AHAM.Services.Commission.API.Application.Modules
{
    public class AutoMapperModule : Module
    {
        private readonly Assembly[] _assembliesToScan;
        private readonly Action<IMapperConfigurationExpression> _mappingConfigurationAction;

        public AutoMapperModule(Assembly[] assembliesToScan, Action<IMapperConfigurationExpression> mappingConfigurationAction)
        {
            this._assembliesToScan = assembliesToScan ?? throw new ArgumentNullException(nameof(assembliesToScan));
            this._mappingConfigurationAction = mappingConfigurationAction ?? throw new ArgumentNullException(nameof(mappingConfigurationAction));
        }

        protected override void Load(ContainerBuilder builder)
        {
            var distinctAssemblies = this._assembliesToScan
                .Where(a => !a.IsDynamic && a.GetName().Name != nameof(AutoMapper))
                .Distinct()
                .ToArray();

            builder.RegisterAssemblyTypes(distinctAssemblies)
                .AssignableTo(typeof(Profile))
                .As<Profile>()
                .SingleInstance();

            builder
                .Register(componentContext =>
                    new MapperConfiguration(config => this.ConfigurationAction(config, componentContext)))
                .As<IConfigurationProvider>()
                .AsSelf()
                .SingleInstance();

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IValueConverter<,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IMappingAction<,>)
            };

            foreach (var openType in openTypes)
                builder.RegisterAssemblyTypes(distinctAssemblies)
                    .AsClosedTypesOf(openType)
                    .AsImplementedInterfaces()
                    .InstancePerDependency();

            builder
                .Register(componentContext => componentContext
                    .Resolve<MapperConfiguration>()
                    .CreateMapper(componentContext.Resolve<IComponentContext>().Resolve))
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }

        private void ConfigurationAction(IMapperConfigurationExpression cfg, IComponentContext componentContext)
        {
            this._mappingConfigurationAction.Invoke(cfg);

            var profiles = componentContext.Resolve<IEnumerable<Profile>>();

            foreach (var profile in profiles)
                cfg.AddProfile(profile);
        }
    }
}