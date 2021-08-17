using System;
using AHAM.Services.Investor.API.Application.Modules;
using AHAM.Services.Investor.API.Extensions;
using AHAM.Services.Investor.API.Services;
using Autofac;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AHAM.Services.Investor.API
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
                .AddCustomDbContext(Configuration)
                .AddDistributedCache(Configuration)
                .AddCustomSwagger(Configuration)
                .AddCustomIntegrations(Configuration)
                .AddCustomConfiguration(Configuration)
                .AddEventBus(Configuration);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAutoMapper(typeof(Startup).Assembly);
            builder.RegisterModule(new MediatorModule());
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
                    "Investor.API V1");
                //OAuth TODO
            });
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            //app.UseCustomAuthentication(Configuration); TODO
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<InvestorService>();
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
