using Confluent.Kafka;
using iCat.MQ.Abstraction.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace iCat.MQ.Kafka.Implements
{
    /// <summary>
    /// Publisher for Kafka, implements <see cref="BasePublisher"/>
    /// </summary>
    public class Publisher<TKey> : BasePublisher
    {
        private readonly IProducer<TKey, string> _producer;

        /// <summary>
        /// Creates a new instance of the Publisher class.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="config"></param>
        public Publisher(string category, ProducerConfig config) : base(category)
        {
            _producer = new ProducerBuilder<TKey, string>(config).Build();

        }

        /// <summary>
        /// <see cref="BasePublisher.SendAsync{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task SendAsync<T>(T data)
        {
            var routeName = base.GetRouteName(data);
            await _producer.ProduceAsync(routeName, new Message<TKey, string>
            {
                Key = (data.Key != null ? (TKey)(object)data.Key! : default)!,
                Value = JsonSerializer.Serialize(data)
            });
        }
    }
}
