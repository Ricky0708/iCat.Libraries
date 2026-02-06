using Confluent.Kafka;
using Confluent.Kafka.Admin;
using iCat.MQ.Abstraction.Attributes;
using iCat.MQ.Abstraction.Models;
using iCat.MQ.Kafka.Implements;
using iCat.MQ.Kafka.Models;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ConsoleTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //        var config = new AdminClientConfig
            //        {
            //            BootstrapServers = "b-2.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092,b-1.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092",
            //        };

            //        using var adminClient = new AdminClientBuilder(config).Build();

            //        var topicName = "user.created";
            //        var topicSpec = new TopicSpecification
            //        {
            //            Name = topicName,
            //            NumPartitions = 2,
            //            ReplicationFactor = 2
            //        };

            //        await adminClient.CreateTopicsAsync(new[]
            //        {
            //    topicSpec
            //});
            var cts = new CancellationTokenSource();

            var publisher = new Publisher<string>("default", new Confluent.Kafka.ProducerConfig
            {
                BootstrapServers = "b-2.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092,b-1.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092",
                MessageMaxBytes = 1000000,
                SocketTimeoutMs = 60000,
                RequestTimeoutMs = 5000,
                ReceiveMessageMaxBytes = 1000000,
                CompressionType = CompressionType.Gzip,
                CompressionLevel = 5,
            }, null, cts.Token);
            var testData = new TestData
            {
                Key = "AuthController",
                Data = "AAAAAA",
                TraceId = "Id"
            };
            var s = JsonSerializer.Serialize(testData);
            await publisher.SendAsync(testData);

            publisher.SendAsyncReliable("user.created", "A", "B");

            var sub = new ConsumerBuilder<string, string>(new ConsumerConfig
            {
                BootstrapServers = "b-2.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092,b-1.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092",
                GroupId = "test-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                EnablePartitionEof = true,
                StatisticsIntervalMs = 5000,
            }).Build();
            sub.Subscribe("user.created");
            while (true)
            {
                var consumeResult = sub.Consume();
                if (!consumeResult.IsPartitionEOF)
                {
                    Console.WriteLine(consumeResult.Message.Value);
                }

            }

            Console.WriteLine("Hello, World!");
        }
    }

    [MessageRoute("user.created")]
    public class TestData : KafkaMQDataModel
    {
        /// <summary>
        /// Data for the message, it can be any string data that you want to send, it will be serialized to JSON before sending. You can also add additional properties if needed.
        /// </summary>
        public string Data { get; set; } = "";
    }
}
