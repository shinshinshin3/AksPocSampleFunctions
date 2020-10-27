using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AzureUtils;
using Microsoft.ApplicationInsights.Channel;
/* Server Telemetry Channel用
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.ApplicationInsights.Extensibility;
*/

[assembly: FunctionsStartup(typeof(AksPocSampleFunctions.Startup))]

namespace AksPocSampleFunctions
{
    public class Startup : FunctionsStartup
    {

        //FunctionsStartup
        //NOTE: https://docs.microsoft.com/ja-jp/azure/azure-functions/functions-dotnet-dependency-injection
        //NOTE: https://blog.shibayan.jp/entry/20200823/1598186591
        //NOTE: https://stackoverflow.com/questions/57564396/how-do-i-mix-custom-parameter-binding-with-dependency-injection-in-azure-functio

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();
            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            IWebJobsBuilder hBuilder = builder.Services.AddWebJobs(x => { return; });
            hBuilder.AddKafka();
            builder.Services.AddLogging();
            builder.Services.AddLogging(loggingBuilder =>
            {
                //loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                loggingBuilder.AddConsole();
                loggingBuilder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Category", LogLevel.Information);
            }).BuildServiceProvider();

            /*
            Server TelemetryChannelではログがApplication Insightsに記録されなかった
            builder.Services.AddSingleton(typeof(ITelemetryChannel),
                                new ServerTelemetryChannel() { StorageFolder = "./" });
            */

            // InMemoryChannelで実装
            builder.Services.AddSingleton(typeof(ITelemetryChannel), new InMemoryChannel() { MaxTelemetryBufferCapacity = 19898 });

            // InstrumentationKeyをappsettings.jsonか環境変数から取得(環境変数が優先)
            var InstrumentationKey = builder.GetContext().Configuration.GetValue<string>("ApplicationInsights_InstrumentationKey");
            var telemetryConfigutaion = new TelemetryClientConfigure(InstrumentationKey).aiOptions;

            builder.Services.AddSingleton<Functions>();
            builder.Services.AddApplicationInsightsTelemetryWorkerService(telemetryConfigutaion);
            builder.Services.BuildServiceProvider(true);
        }
    }
}
