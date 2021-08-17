using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;

namespace WebStatus
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace;

        public static async Task Main(string[] args)
        {
            var configuration = Configuration();

            Log.Logger = CreateLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})", AppName);
                var host = CreateHostBuilder(configuration, args).Build();
                
                LogPackagesVersionInfo();
                
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
                .CaptureStartupErrors(false).UseStartup<Startup>())
            .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
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

        private static void LogPackagesVersionInfo()
        {
            var assemblies = new List<Assembly>();
            foreach (var dependency in typeof(Program).Assembly.GetReferencedAssemblies())
            {
                try
                {
                    assemblies.Add(Assembly.Load(dependency));
                }
                catch
                {
                    //Skip fail loaded assembly
                }
            }

            var versionList = assemblies.Select(a => $"-{a.GetName().Name} - {GetVersion(a)}").OrderBy(value => value);
            Log.Logger.ForContext("PackageVersions", string.Join("\n", versionList)).Information("Package versions ({ApplicationContext})", AppName);
        }

        private static string GetVersion(Assembly assembly)
        {
            try
            {
                return $"{assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version} ({assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split()[0]})";
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}