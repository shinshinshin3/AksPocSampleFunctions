using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.Kafka;
using Microsoft.Extensions.DependencyInjection;
using consumerFunction01.common;
using Microsoft.Extensions.Configuration;
using consumerFunction01.DataModel;

namespace consumerFunction01
{
    public static class consumeFunction01
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                  //.UseEnvironment("Development"
                  .ConfigureAppConfiguration(b =>
                  {
                      b.AddJsonFile("appsettings.json");
                  })
                  .ConfigureHostConfiguration(b =>
                  {
                      b.AddEnvironmentVariables();
                  })
                  .ConfigureWebJobs(b =>
                  {
                      b.AddKafka();
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
        const string Broker = "127.0.0.1:32770";
        const string Topic = "wng";
        //private string Broker = Environment.GetEnvironmentVariable("broker");
        //private string Topic = Environment.GetEnvironmentVariable("topic");
        //private string StringTopicWithOnePartition = Environment.GetEnvironmentVariable("wng");
        private static Logger log = Logger.GetInstance(String.Format("./logfile_{0}", DateTime.UtcNow.ToString("yyyyMMddHHmm")), true);


        [FunctionName("Functions")]
        public static void MultiItemTriggerTenPartitions(
            [KafkaTrigger(Broker, Topic, ConsumerGroup = "myConsumerGroup")] KafkaEventData<string>[] events
            //[Table("kafkaExtensionTable02")] ICollector<ConsumerResult> outputTable,
            //MyLogger
            //Logger log
            )
        {
            //log.Info("function start");
            var rets = new System.Collections.Generic.List<ConsumerResult>();
            foreach (var kafkaEvent in events)
            {
                var topicData = JsonConvert.DeserializeObject<Wng>(kafkaEvent.Value);
                var now = DateTime.UtcNow;
                var consumerResult = new ConsumerResult()
                {
                    PartitionKey = GetInstanceName(),
                    //RowKey = Guid.NewGuid().ToString(),
                    RowKey = topicData.TransactionId.ToString().PadLeft(8, '0'),
                    consumeTime = now.ToString("yyyy-MM-dd-HH:mm:ss.fff"),
                    timespan = (now - topicData.OccurrenceDate),
                    partition = kafkaEvent.Partition,
                    topic = kafkaEvent.Topic,
                    topicTime = kafkaEvent.Timestamp.ToString("yyyy-MM-dd-HH:mm:ss.fff"),
                    offset = kafkaEvent.Offset.ToString()
                };
                log.Info(JsonConvert.SerializeObject(consumerResult));

                log.Info(kafkaEvent.Value.Substring(0, 100));
                /*
                try
                {
                    outputTable.Add(consumerResult);
                }
                catch (Exception e)
                {
                    log.LogInformation(e.Message);
                }
                */
                //log.Info("function end");
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

