using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.Storage;

namespace consumerFunction01
{
    public static class consumeFunction01
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                  .UseEnvironment("Development")
                  .ConfigureWebJobs(b =>
                  {
                      b.AddKafka();
                  })
                  .ConfigureAppConfiguration(b =>
                  {
                  })
                  .ConfigureLogging((context, b) =>
                  {
                      b.SetMinimumLevel(LogLevel.Debug);
                      //b.AddConsole();
                  })
                  .ConfigureServices(services =>
                  {
                      services.AddSingleton<Functions>();
                  })
                  .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }

    public class Functions
    {
        const string Broker = "10.0.3.8:9092";
        const string StringTopicWithOnePartition = "confluent";
        //const string StringTopicWithTenPartitions = "stringTopicTenPartitions";
        /// <summary>
        /// Trigger for the topic
        /// </summary>
        /// 
        [FunctionName("Functions")]
        //[return:] Table("kafkaExtensionTable02", Connection = "StorageConnectionAppSetting")
        public void MultiItemTriggerTenPartitions(
            [KafkaTrigger(Broker, StringTopicWithOnePartition, ConsumerGroup = "myConsumerGroup")] KafkaEventData<string>[] events,
            [Table("kafkaExtensionTable02")] ICollector<ConsumerResult> outputTable,
            ILogger log)
        {
            var rets = new System.Collections.Generic.List<ConsumerResult>();
            foreach (var kafkaEvent in events)
            {
                var topicData = JsonConvert.DeserializeObject<TopicData>(kafkaEvent.Value);
                var now = DateTime.UtcNow;
                var consumerResult = new ConsumerResult()
                {
                    PartitionKey = GetInstanceName(),
                    //RowKey = Guid.NewGuid().ToString(),
                    RowKey = topicData.id.ToString().PadLeft(8, '0'),
                    cunsumeTime = now.ToString("yyyy-MM-dd-HH:mm:ss.fff"),
                    timespan = (now - topicData.date),
                    partition = kafkaEvent.Partition,
                    topic = kafkaEvent.Topic,
                    topicTime = kafkaEvent.Timestamp.ToString("yyyy-MM-dd-HH:mm:ss.fff"),
                    offset = kafkaEvent.Offset.ToString()
                };
                log.LogInformation(JsonConvert.SerializeObject(consumerResult));
                try
                {
                    outputTable.Add(consumerResult);
                }
                catch (Exception e)
                {
                    log.LogInformation(e.Message);
                }
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
        public class TopicData
        {
            public DateTime date { get; set; }
            public int id { get; set; }
        }

        public class ConsumerResult
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public TimeSpan timespan { get; set; }
            public string cunsumeTime { get; set; }
            public string topic { get; set; }
            public string topicTime { get; set; }
            public int partition { get; set; }
            public string offset { get; set; }
        }
    }
}

