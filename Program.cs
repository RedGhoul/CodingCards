using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodingCards.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sentry;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace CodingCards
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration configuration = null;
            try
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            using (SentrySdk.Init(Secrets.GetConnectionString(configuration, "Sentry_URL")))
            {
                Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                    new Uri($"{Secrets.GetConnectionString(configuration, "Log_ElasticIndexBaseUrl")}"))
                {
                    AutoRegisterTemplate = true,
                    ModifyConnectionSettings = x =>
                             x.BasicAuthentication(
                                 Secrets.GetAppSettingsValue(configuration, "Elastic_Logging_UserName"),
                                 Secrets.GetAppSettingsValue(configuration, "Elastic_Logging_Password")),
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    IndexFormat = $"{Secrets.GetAppSettingsValue(configuration, "AppName")}" + "-{0:yyyy.MM}"
                })
               .CreateLogger();

                try
                {
                    Log.Information("Starting up");
                    CreateWebHostBuilder(args).Build().Run();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Application start-up failed");
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }
           

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
