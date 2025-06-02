using Confluent.Kafka;
using iCat.MQ.Abstraction.Attributes;
using iCat.MQ.Abstraction.Models;
using iCat.MQ.Kafka.Implements;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ConsoleTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var publisher = new Publisher<string>("default", new Confluent.Kafka.ProducerConfig
            {
                BootstrapServers = "b-2.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092,b-1.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092",
                MessageMaxBytes = 1000000,
                SocketTimeoutMs = 60000,
                RequestTimeoutMs = 5000,
                ReceiveMessageMaxBytes = 1000000,
                CompressionType = CompressionType.Gzip,
                CompressionLevel = 5,
            });


            await publisher.SendAsync(new TestData
            {
                Key = "AuthController",
                TopicName = "user.created",
                Data = "AAAAAA",
                TraceId = "Id"
            });

            var sub = new ConsumerBuilder<string, string>(new ConsumerConfig
            {
                BootstrapServers = "b-2.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092,b-1.mskdev.roksss.c3.kafka.ap-southeast-1.amazonaws.com:9092",
                GroupId = "test-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                EnablePartitionEof = true,
                StatisticsIntervalMs = 5000,
            }).Build();
            //sub.Subscribe("user.created");
            //while (true)
            //{
            //    var consumeResult = sub.Consume();
            //    if (!consumeResult.IsPartitionEOF)
            //    {
            //        Console.WriteLine(consumeResult.Message.Value);
            //    }

            //}

            Console.WriteLine("Hello, World!");
        }
    }

    [MessageRoute("user.updated")]
    public class TestData : BaseMQDataModel
    {
        public string TopicName { get; set; }

        public string Key { get; set; }

        public string Data { get; set; }
    }
}
