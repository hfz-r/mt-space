using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AHAM.Services.Investor.API.Application.Modules;
using Autofac;
using AutoMapper;

namespace AHAM.Services.Investor.API.Extensions
{
    public static class ContainerBuilderExtensions
    {
        private static readonly Action<IMapperConfigurationExpression> FallBackExpression =
            config => { };

        public static ContainerBuilder RegisterAutoMapper(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            return RegisterAddAutoMapperInternal(builder, assemblies);
        }

        public static ContainerBuilder RegisterAutoMapper(this ContainerBuilder builder, Assembly assembly)
        {
            return RegisterAddAutoMapperInternal(builder, new[] { assembly });
        }

        public static ContainerBuilder RegisterAutoMapper(this ContainerBuilder builder,
            Action<IMapperConfigurationExpression> configExpression, params Assembly[] assemblies)
        {
            return RegisterAddAutoMapperInternal(builder, assemblies, configExpression);
        }

        public static ContainerBuilder RegisterAutoMapper(this ContainerBuilder builder,
            Action<IMapperConfigurationExpression> configExpression, Assembly assembly)
        {
            return RegisterAddAutoMapperInternal(builder, new[] { assembly }, configExpression);
        }

        private static ContainerBuilder RegisterAddAutoMapperInternal(ContainerBuilder builder,
            IEnumerable<Assembly> assemblies, Action<IMapperConfigurationExpression> configExpression = null)
        {
            var usedAssemblies = assemblies as Assembly[] ?? assemblies.ToArray();

            var usedConfigExpression = configExpression ?? FallBackExpression;

            builder.RegisterModule(new AutoMapperModule(usedAssemblies, usedConfigExpression));

            return builder;
        }
    }
}