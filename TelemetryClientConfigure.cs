using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureUtils
{
    public class TelemetryClientConfigure
    {
        public ApplicationInsightsServiceOptions aiOptions;
        
        public TelemetryClientConfigure(string instrumentationKey)
        {
            aiOptions = new ApplicationInsightsServiceOptions();
            aiOptions.InstrumentationKey = instrumentationKey;
            setTelemetryClientOptions();
        }
        private void setTelemetryClientOptions()
        {
            //aiOptions.InstrumentationKey = _configuration.GetValue<string>("ApplicationInsights_InstrumentationKey");

            // Disables adaptive sampling. 
            aiOptions.EnableAdaptiveSampling = false;

            // Collects Requests Telemetry
            //aiOptions.EnableRequestTrackingTelemetryModule = true;
            // よくわからんけど有効
            aiOptions.EnableEventCounterCollectionModule = true;
            // Collects Depdndency Telemetry
            aiOptions.EnableDependencyTrackingTelemetryModule = true;
            // Disables QuickPulse (Live Metrics stream).
            aiOptions.EnableQuickPulseMetricStream = false;
        }
    }
}