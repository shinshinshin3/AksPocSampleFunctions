using System;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;

namespace AksPocSampleFunctions
{
    public class Functions
    {
        private TelemetryClient _telemetryClient;
        private readonly IConfiguration _configuration;
        public Functions(IConfiguration configuration, TelemetryClient tc)
        {
            _configuration = configuration;
            _telemetryClient = tc;
        }

        // "%BROKER_LIST%": this paramater's value is getting by Environment $BROKER_LIST   
        [FunctionName("Functions")]
        public void MultiItemTriggerTenPartitions(
            [KafkaTrigger("%BROKER_LIST%", "%TOPIC_NAME%", ConsumerGroup = "%CONSUMER_GROUP%")]
            KafkaEventData<string>[] events
            /* Azure Functionsで動作させるときはこれを使う。
            ,ILogger log
             */
            )
        {
            _telemetryClient.TrackTrace("function start", SeverityLevel.Information);

            var rets = new System.Collections.Generic.List<ConsumerResult>();
            foreach (var kafkaEvent in events)
            {
                var topicData = JsonConvert.DeserializeObject<Wng>(kafkaEvent.Value);
                _telemetryClient.TrackTrace(String.Format("OccurenceDate: {0}, TransactionId: {1}", topicData.OccurrenceDate, topicData.TransactionId), SeverityLevel.Information);

                var now = DateTime.UtcNow;
                var consumerResult = new ConsumerResult()
                {
                    PartitionKey = GetInstanceName(),
                    //RowKey = Guid.NewGuid().ToString(),
                    //RowKey = topicData.TransactionId.ToString().PadLeft(8, '0'),
                    RowKey = topicData.TransactionId,
                    consumeTime = now.ToString("yyyy-MM-dd-HH:mm:ss.fff"),
                    timespan = (now - topicData.OccurrenceDate),
                    partition = kafkaEvent.Partition,
                    topic = kafkaEvent.Topic,
                    topicTime = kafkaEvent.Timestamp.ToString("yyyy-MM-dd-HH:mm:ss.fff"),
                    offset = kafkaEvent.Offset.ToString()
                };

                _telemetryClient.TrackTrace(JsonConvert.SerializeObject(consumerResult));
            }

            string GetInstanceName()
            {
                var hostname = Environment.GetEnvironmentVariable("COMPUTERNAME") ?? string.Empty;
                if (string.IsNullOrEmpty(hostname))
                {
                    hostname = Environment.MachineName;
                }
                return hostname;
            }
        }

        public class ConsumerResult
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public TimeSpan timespan { get; set; }
            public string consumeTime { get; set; }
            public string topic { get; set; }
            public string topicTime { get; set; }
            public int partition { get; set; }
            public string offset { get; set; }
        }
    }
}
