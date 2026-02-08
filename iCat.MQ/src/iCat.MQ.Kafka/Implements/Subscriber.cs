using Confluent.Kafka;
using iCat.MQ.Abstraction.Abstractions;
using iCat.MQ.Kafka.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace iCat.MQ.Kafka.Implements
{
    /// <summary>
    /// Subscriber for Kafka, implements <see cref="BaseSubscriber"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class Subscriber : BaseSubscriber
    {
        private readonly Models.ConsumerConfig _config;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _ctx;
        private readonly ConcurrentHashSet<string> _set = new ConcurrentHashSet<string>();
        private delegate T delgExecuter<T>(string message);
        private bool _disposed = false;

        /// <summary>
        /// Creates a new instance of the Subscriber class.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        public Subscriber(string category, Models.ConsumerConfig config, ILogger<Subscriber> logger, CancellationToken cancellationToken) : base(category, cancellationToken)
        {
            _config = config;
            _logger = logger;
            _ctx = new CancellationTokenSource();
            _cancellationToken.Register(() => _ctx.Cancel());
        }

        public override Task Subscribe<T>(string messageGroup, Action<T> processReceived)
        {
            if (!(_config.EnableAutoCommit ?? true)) throw new Exception("The instance doesn't supports auto-commit, please use Subscribe<T>(string messageGroup, Func<T, bool> processReceived) instead.");
            var lambda = BuildDelg<T>();
            return SubscribeCore<T>(messageGroup, (consumer, result) =>
            {
                processReceived(lambda.Invoke(result.Message.Value));
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageGroup"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override Task Subscribe<T>(string messageGroup, Func<T, bool> processReceived)
        {
            if (_config.EnableAutoCommit ?? true) throw new Exception("The instance support auto-commit, please use Subscribe<T>(string messageGroup, Action<T> processReceived) instead.");
            var lambda = BuildDelg<T>();
            return SubscribeCore<T>(messageGroup, (consumer, result) =>
            {
                if (processReceived(lambda.Invoke(result.Message.Value)))
                {
                    consumer.Commit(result);
                }
                ;
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageGroup"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override Task SubscribeToString<T>(string messageGroup, Action<string> processReceived)
        {
            if (!(_config.EnableAutoCommit ?? true)) throw new Exception("The instance doesn't supports auto-commit, please use SubscribeToString<T>(string messageGroup, Func<string, bool> processReceived) instead.");
            var lambda = BuildDelg<T>();
            return SubscribeCore<T>(messageGroup, (consumer, result) =>
            {
                processReceived(result.Message.Value);
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageGroup"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override Task SubscribeToString<T>(string messageGroup, Func<string, bool> processReceived)
        {
            if (_config.EnableAutoCommit ?? true) throw new Exception("The instance support auto-commit, please use SubscribeToString<T>(string messageGroup, Action<string> processReceived) instead.");
            var lambda = BuildDelg<T>();
            return SubscribeCore<T>(messageGroup, (consumer, result) =>
            {
                if (processReceived(result.Message.Value))
                {
                    consumer.Commit(result);
                }
                ;
            });
        }

        private async Task SubscribeCore<T>(string messageGroup, Action<IConsumer<string, string>, ConsumeResult<string, string>> process)
        {
            var topic = base.GetRouteName(typeof(T));
            if (_set.Add($"{topic}-{messageGroup}"))
            {
                await Common.EnsureTopicAsync(_config.BootstrapServers, topic, _config.Partitions, _config.ReplicationFactor);
                _config.GroupId = messageGroup;
                var consumer = new ConsumerBuilder<string, string>(_config).Build();

                _ = Task.Run(() =>
                {
                    consumer.Subscribe(topic);
                    while (!_ctx.Token.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(_ctx.Token);
                            if (!consumeResult.IsPartitionEOF)
                            {
                                process(consumer, consumeResult);
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            _logger?.LogError(ex, "Consume failed");
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                    }
                    consumer.Close();
                    consumer.Dispose();
                }).ContinueWith(t => _logger.LogError(t.Exception, "Consume Task failed"), TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private static delgExecuter<T> BuildDelg<T>()
        {
            // 定義 target 的型別
            var targetExpr = Expression.Parameter(typeof(string), "target");

            var deserializeMethodInfo = typeof(JsonSerializer).GetMethod("Deserialize", new[] { typeof(string), typeof(JsonSerializerOptions) })!;
            MethodInfo genericMethod = deserializeMethodInfo.MakeGenericMethod(new[] { typeof(T) });
            // 將此呼叫 Deserialize(target, Convert(null, JsonSerializerOptions)) 做成 Expression
            var jsonSerializerExpr = Expression.Call(genericMethod, targetExpr, Expression.Convert(Expression.Constant(null), typeof(JsonSerializerOptions)));

            // 完成 expression → (instance, target) => instance./MethodName/.Invoke(Deserialize(target, Convert(null, JsonSerializerOptions)))
            var lambdaExpr = Expression.Lambda<delgExecuter<T>>(jsonSerializerExpr, targetExpr);

            // 編譯
            var lambda = lambdaExpr.Compile();

            return lambda;
        }

        #region dispose

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
                _ctx.Cancel();
            }
        }

        #endregion
    }
}
