using Confluent.Kafka;
using iCat.MQ.Abstraction.Abstractions;
using iCat.MQ.Kafka.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace iCat.MQ.Kafka.Implements
{
    public class Subscriber<TKey> : BaseSubscriber
    {
        private readonly ConcurrentDictionary<string, ConsumerInfo<TKey>> _consumers;
        private readonly ConcurrentDictionary<string, Delegate> _delegates;
        private readonly ConsumerConfig _config;
        public Subscriber(string category, ConsumerConfig config) : base(category)
        {
            _config = config;
        }

        public override Task Subscribe<T>(string messageGroup, Action<T> processReceived)
        {
            if (_config.EnableAutoCommit ?? true) throw new Exception("The instance does not support auto-acknowledge, please use Subscribe<T>(string messageGroup, Func<T, bool> processReceived) instead.");
            return SubscribeCore<T>(messageGroup, processReceived);
        }

        public override Task Subscribe<T>(string messageGroup, Func<T, bool> processReceived)
        {
            if (!(_config.EnableAutoCommit ?? true)) throw new Exception("The instance supports auto-acknowledge, please use Subscribe<T>(string messageGroup, Action<T> processReceived) instead.");
            return SubscribeCore<T>(messageGroup, processReceived);
        }

        public override Task SubscribeToString<T>(string messageGroup, Action<string> processReceived)
        {
            if (_config.EnableAutoCommit ?? true) throw new Exception("The instance does not support auto-acknowledge, please use Subscribe<T>(string messageGroup, Func<T, bool> processReceived) instead.");
            return SubscribeCore<T>(messageGroup, processReceived);
        }

        public override Task SubscribeToString<T>(string messageGroup, Func<string, bool> processReceived)
        {
            if (!(_config.EnableAutoCommit ?? true)) throw new Exception("The instance supports auto-acknowledge, please use Subscribe<T>(string messageGroup, Action<T> processReceived) instead.");
            return SubscribeCore<T>(messageGroup, processReceived);
        }

        private Task SubscribeCore<T>(string messageGroup, Delegate processReceived)
        {
            if (_config.EnableAutoCommit ?? true) throw new Exception("The instance does not support auto-acknowledge, please use Subscribe<T>(string messageGroup, Func<T, bool> processReceived) instead.");

            var topic = base.GetRouteName(typeof(T));
            if (!_consumers.TryGetValue(messageGroup, out var consumer))
            {
                lock (_consumers)
                {
                    if (!_consumers.TryGetValue(messageGroup, out consumer))
                    {
                        consumer = new ConsumerInfo<TKey>
                        {
                            Consumer = new ConsumerBuilder<TKey, string>(_config).Build(),
                        };
                    }
                }
            }
            consumer.Topics.AddOrUpdate(topic, processReceived, (name, old) => processReceived);
            return Task.CompletedTask;
        }
    }
}
