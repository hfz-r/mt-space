using AHAM.BuildingBlocks.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AHAM.Services.Investor.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCustomAuthentication(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        public static void UseEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //eventBus.Subscribe<IIntegrationEventHandler<>>(); TODO
        }
    }
}