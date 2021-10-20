using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Threading.Tasks;
using MassTransit.EntityFrameworkCoreIntegration.JobService;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitJobConsumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = CreateLogger();
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                Log.Logger.Information("Application starting up...");
                var dbContext = services.GetRequiredService<JobServiceSagaDbContext>();
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.EnsureCreatedAsync();
                await host.RunAsync();
            }
            catch(Exception ex)
            {
                Log.Logger.Fatal(ex, "Application startup failed.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog();
        }

        public static Serilog.ILogger CreateLogger()
        {
            var configuration = LoadAppConfiguration();
            var elasticSearchUri = configuration.GetValue("ELASTIC_URI", "http://localhost:9200");
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("JobService", Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .ReadFrom.Configuration(configuration)
                .Destructure.AsScalar<JObject>()
                .Destructure.AsScalar<JArray>()
                .Enrich.FromLogContext()
                .Enrich.FromMassTransit()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchUri))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                        IndexFormat = "masstransitjobconsumer-{0:yyyy.MM}",
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true)
                    })
                .CreateLogger();
        }

        private static IConfigurationRoot LoadAppConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
