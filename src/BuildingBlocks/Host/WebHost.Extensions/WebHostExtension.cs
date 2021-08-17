using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostExtensions
    {
        public static bool IsInKubernetes(this IHost host)
        {
            var configuration = host.Services.GetService<IConfiguration>();
            var orchestratorType = configuration.GetValue<string>("ORCHESTRATOR_TYPE");

            return orchestratorType?.ToUpper() == "K8S";
        }

        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            var underK8S = host.IsInKubernetes();

            using var scope = host.Services.CreateScope();

            var serviceProvider = scope.ServiceProvider;
            var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
            var context = serviceProvider.GetService<TContext>();
            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                if (underK8S) InvokeSeeder(seeder, context, serviceProvider);
                else
                {
                    var retries = 10;
                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(retryCount: retries,
                            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (exception, timeSpan, r, ctx) =>
                            {
                                logger.LogWarning(exception,
                                    "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                                    nameof(TContext), exception.GetType().Name, exception.Message, r, retries);
                            });
                    retry.Execute(() => InvokeSeeder(seeder, context, serviceProvider));
                }

                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                if (underK8S) throw;
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
            IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}