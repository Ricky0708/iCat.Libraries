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
    public class Publisher<TKey> : BasePublisher, IDisposable
    {
        private readonly IProducer<TKey, string> _producer;
        private readonly ConcurrentQueue<KafkaMQDataModel> _queue = new ConcurrentQueue<KafkaMQDataModel>();
        private readonly Task _task;
        private readonly ILogger _logger;
        private bool _disposed = false;
        private CancellationTokenSource _ctx;
        private int _retryCount = 0;

        /// <summary>
        /// Creates a new instance of the Publisher class.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        public Publisher(string category, ProducerConfig config, ILogger<Publisher<TKey>> logger, CancellationToken cancellationToken) : base(category)
        {
            _ctx = new CancellationTokenSource();
            _producer = new ProducerBuilder<TKey, string>(config).Build();
            cancellationToken.Register(() => _ctx.Cancel());
            _task = Task.Run(async () =>
            {
                while (!_ctx.Token.IsCancellationRequested || !_queue.IsEmpty)
                {
                    if (_retryCount > 5)
                    {
                        _queue.TryDequeue(out var errorData);
                        _retryCount = 0;
                        logger.LogError($"Produce fail:{JsonSerializer.Serialize(errorData)}");
                    }
                    try
                    {
                        if (!_queue.TryPeek(out var data))
                        {
                            await Task.Delay(3000, _ctx.Token);
                            continue;
                        }

                        var result = await _producer.ProduceAsync(data.Topic, new Message<TKey, string>
                        {
                            Key = (data.Key != null ? (TKey)(object)data.Key! : default)!,
                            Value = JsonSerializer.Serialize(data, data.GetType())
                        });

                        _queue.TryDequeue(out _);
                        _retryCount = 0;
                    }
                    catch (ProduceException<TKey, string> ex)
                    {
                        _retryCount += 1;
                        await Task.Delay(500);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            });
            _logger = logger;
        }

        public void SendAsyncReliable(string topic, TKey key, string value)
        {
            _ = Task.Run(async () =>
            {
                int attempt = 0;
                int maxRetry = 5;

                while (attempt < maxRetry)
                {
                    attempt++;
                    try
                    {
                        // 等待 broker ack
                        var result = await _producer.ProduceAsync(topic, new Message<TKey, string>
                        {
                            Key = key,
                            Value = value
                        });

                        Console.WriteLine($"Message sent to partition {result.Partition}, offset {result.Offset}");
                        break; // 成功送出就跳出
                    }
                    catch (ProduceException<TKey, string> ex)
                    {
                        Console.WriteLine($"Attempt {attempt} failed: {ex.Error.Reason}");
                        await Task.Delay(500); // 等一下再 retry
                    }
                }
            });
        }

        /// <summary>
        /// <see cref="BasePublisher.SendAsync{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task SendAsync<T>(T data)
        {
            _ctx.Token.ThrowIfCancellationRequested();
            var topic = base.GetRouteName(data);
            if (data is KafkaMQDataModel targetModel)
            {
                targetModel.Topic = topic;
                _queue.Enqueue(targetModel);
            }
            else
            {
                throw new InvalidOperationException($"This operation only supports {nameof(KafkaMQDataModel)}.");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Dispose the producer, flush the messages in the queue before disposing.
        /// </summary>
        public void Dispose()
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
                    _task?.Wait();
                    _producer?.Flush(TimeSpan.FromSeconds(60));
                    _producer?.Dispose();
                    _task?.Dispose();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
