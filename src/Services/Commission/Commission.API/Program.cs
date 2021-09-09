using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AHAM.BuildingBlocks.IntegrationEventLog;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AHAM.Services.Commission.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;

        public static readonly string AppName = Namespace[(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1)..];

        public static async Task Main(string[] args)
        {
            var configuration = Configuration();

            Log.Logger = CreateLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})", AppName);
                var host = CreateHostBuilder(configuration, args).Build();

                //todo: (future use - db context)  
                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                host.MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .CaptureStartupErrors(false)
                .ConfigureKestrel(options =>
                {
                    var (httpPort, grpcPort) = Ports(configuration);

                    options.Listen(IPAddress.Any, httpPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
                    options.Listen(IPAddress.Any, grpcPort,
                        listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                })
                .UseStartup<Startup>())
            .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog();

        #region Private methods

        private static IConfiguration Configuration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static ILogger CreateLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logStashUrl = configuration["Serilog:LogStashUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logStashUrl) ? "http://logstash:8080" : logStashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static (int httpPort, int grpcPort) Ports(IConfiguration configuration)
        {
            var httpPort = configuration.GetValue("PORT", 80);
            var grpcPort = configuration.GetValue("GRPC_PORT", 5001);
            return (httpPort, grpcPort);
        }

        #endregion
    }
}