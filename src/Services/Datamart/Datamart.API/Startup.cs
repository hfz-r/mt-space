using System;
using AHAM.Services.Datamart.API.Extensions;
using AHAM.Services.Datamart.API.Infrastructure.Autofac;
using AHAM.Services.Datamart.API.Services;
using Autofac;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AHAM.Services.Datamart.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddGrpc(Configuration)
                .AddApplicationInsights(Configuration)
                .AddCustomMvc()
                .AddCustomHealthCheck(Configuration)
                .AddDistributedCache(Configuration)
                .AddCustomSwagger(Configuration)
                .AddCustomConfiguration(Configuration)
                .AddBusProvider(Configuration)
                .AddEventBus(Configuration);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAutoMapper(typeof(Startup).Assembly);
            builder.RegisterModule(new ApplicationModule());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase)) app.UsePathBase(pathBase);

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : String.Empty)}/apidocs.swagger.json",
                    "Datamart.API V1");
                //OAuth TODO
            });
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            //app.UseCustomAuthentication(Configuration); TODO
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<CimbService>(); 
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = x => x.Name.Contains("self")
                });
            });
            app.UseEventBus();
        }
    }
}
