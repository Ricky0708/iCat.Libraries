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
            var testDataA = new TestDataA
            {
                Key = "AuthController",
                Data = "AAAAAA",
                TraceId = "IdA"
            };

            var testDataB = new TestDataB
            {
                Key = "AuthController",
                Data = "BBBBBB",
                TraceId = "IdB"
            };

            var publisher = new Publisher("default", new iCat.MQ.Kafka.Models.ProducerConfig
            {
                BootstrapServers = "b-2.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092,b-1.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092",
                MessageMaxBytes = 1000000,
                SocketTimeoutMs = 60000,
                RequestTimeoutMs = 5000,
                ReceiveMessageMaxBytes = 1000000,
                CompressionType = CompressionType.Gzip,
                CompressionLevel = 5,
                ReplicationFactor = 2,
                Partitions = 3,
                Acks = Acks.All,
                EnableIdempotence = true,
                MessageTimeoutMs = 10000,
            }, null, cts.Token);


            var sub = new Subscriber("A", new iCat.MQ.Kafka.Models.ConsumerConfig
            {
                BootstrapServers = "b-2.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092,b-1.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092",
                GroupId = "test-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnablePartitionEof = true,
                StatisticsIntervalMs = 5000,
                ReplicationFactor = 2,
                Partitions = 3,
            }, null, cts.Token);

            await sub.Subscribe<TestDataA>("SS", data =>
            {
                Console.WriteLine(data.Data);
                return true;
            });
            //await sub.Subscribe<TestDataB>("SS", data =>
            //{
            //    Console.WriteLine(data.Data);
            //    return true;
            //});

            //while (true)
            //{
            //    var consumeResult = sub.Consume();
            //    if (!consumeResult.IsPartitionEOF)
            //    {
            //        Console.WriteLine(consumeResult.Message.Value);
            //    }
            //    sub.Commit(consumeResult);

            //}

            Console.WriteLine("Hello, World!");
            while (true)
            {
                var line = Console.ReadLine();
                if (line == "1") await publisher.SendAsync(testDataA);
                if (line == "2") await publisher.SendAsync(testDataB);
                if (line == "3") await cts.CancelAsync();

            }
        }
    }

    [MessageRoute("test.topic.a")]
    public class TestDataA : KafkaMQDataModel
    {
        /// <summary>
        /// Data for the message, it can be any string data that you want to send, it will be serialized to JSON before sending. You can also add additional properties if needed.
        /// </summary>
        public string Data { get; set; } = "";
    }

    [MessageRoute("test.topic.b")]
    public class TestDataB : KafkaMQDataModel
    {
        /// <summary>
        /// Data for the message, it can be any string data that you want to send, it will be serialized to JSON before sending. You can also add additional properties if needed.
        /// </summary>
        public string Data { get; set; } = "";
    }
}
