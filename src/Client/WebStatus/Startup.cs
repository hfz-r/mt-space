using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace WebStatus
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
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddControllers();
            services.AddOptions();
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
            services.AddHealthChecksUI(setup => setup.SetHeaderText("AHAM MT-Space - Health Checks Status"))
                .AddInMemoryStorage();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseExceptionHandler("/Home/Error");

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase)) app.UsePathBase(pathBase);

            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = x => x.Name.Contains("self")
                });
                endpoints.MapHealthChecksUI(options =>
                {
                    options.ResourcesPath = string.IsNullOrEmpty(pathBase) ? "/ui/resources" : $"{pathBase}/ui/resources";
                    options.UIPath = "/hc-ui";
                    options.AddCustomStylesheet(Path.Combine(env.WebRootPath, "css/hc.css"));
                });
            });
        }
    }
}