using Confluent.Kafka;
using iCat.MQ.Abstraction.Abstractions;
using iCat.MQ.Abstraction.Models;
using iCat.MQ.Kafka.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace iCat.MQ.Kafka.Implements
{
    /// <summary>
    /// Publisher for Kafka, implements <see cref="BasePublisher"/>
    /// </summary>
    public class Publisher : BasePublisher
    {
        private readonly IProducer<string?, string> _producer;
        private readonly Models.ProducerConfig _config;
        private readonly ILogger _logger;
        private bool _disposed = false;
        private readonly CancellationTokenSource _ctx;
        private readonly ConcurrentHashSet<string> _set = new ConcurrentHashSet<string>();

        /// <summary>
        /// Creates a new instance of the Publisher class.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        public Publisher(string category, Models.ProducerConfig config, ILogger<Publisher> logger, CancellationToken cancellationToken) : base(category)
        {
            _producer = new ProducerBuilder<string?, string>(config).Build();
            _config = config;
            _logger = logger;
            _ctx = new CancellationTokenSource();
            cancellationToken.Register(() => _ctx.Cancel());
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
            _ctx.Token.ThrowIfCancellationRequested();
            var topic = base.GetRouteName(data);

            if (!_set.Contains(topic))
            {
                await Common.EnsureTopicAsync(_config.BootstrapServers, topic, _config.Partitions, _config.ReplicationFactor);
                _set.Add(topic);
            }

            if (data is KafkaMQDataModel targetModel)
            {
                var result = await _producer.ProduceAsync(topic, new Message<string?, string>
                {
                    Key = targetModel.Key,
                    Value = JsonSerializer.Serialize(data, data.GetType())
                });
            }
            else
            {
                throw new InvalidOperationException($"This operation only supports {nameof(KafkaMQDataModel)}.");
            }
        }

        /// <summary>
        /// Dispose the producer, flush the messages in the queue before disposing.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the producer, flush the messages in the queue before disposing.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                try
                {
                    _ctx.Cancel();
                    _producer?.Flush(TimeSpan.FromSeconds(60));
                    _producer?.Dispose();
                    _ctx.Dispose();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
