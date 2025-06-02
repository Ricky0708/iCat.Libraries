using iCat.MQ.Abstraction.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace iCat.MQ.RabbitMQ.Implements
{
    /// <summary>
    /// Subscriber for RabbitMQ, implements <see cref="BaseSubscriber"/>
    /// </summary>
    public class Subscriber : BaseSubscriber
    {
        private readonly IConnection _connection;
        private readonly string _prefix;
        private readonly bool _isAutoDeleteQueue;

        private delegate T delgExecuter<T>(string message);
        private readonly ConcurrentDictionary<string, IModel> _models;
        private readonly ConcurrentDictionary<string, string> _consumeTags;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="category"></param>
        /// <param name="prefix"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Subscriber(IConnection connection, string category, string prefix, bool isAutoDeleteQueue) : base(category)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._prefix = prefix;
            _isAutoDeleteQueue = isAutoDeleteQueue;
            _models = new ConcurrentDictionary<string, IModel>();
            _consumeTags = new ConcurrentDictionary<string, string>();
        }

        #region public methods

        /// <inheritdoc/>
        public override Task Subscribe<T>(string queueName, Action<T> processReceived)
        {
            var exchangeName = base.GetRouteName(typeof(T));
            var lambda = BuildDelg<T>();
            SubscribeCore(exchangeName, queueName, _isAutoDeleteQueue, true, (channel, ea) =>
            {
                processReceived(lambda.Invoke(Encoding.UTF8.GetString(ea.Body.ToArray())));
            });
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task Subscribe<T>(string queueName, Func<T, bool> processReceived)
        {
            var exchangeName = GetRouteName(typeof(T));
            var lambda = BuildDelg<T>();
            SubscribeCore(exchangeName, queueName, _isAutoDeleteQueue, false, (channel, ea) =>
            {
                if (processReceived(lambda.Invoke(Encoding.UTF8.GetString(ea.Body.ToArray()))))
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
                ;
            });
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task SubscribeToString<T>(string queueName, Action<string> processReceived)
        {
            var exchangeName = GetRouteName(typeof(T));
            SubscribeCore(exchangeName, queueName, _isAutoDeleteQueue, true, (channel, ea) =>
            {
                processReceived(Encoding.UTF8.GetString(ea.Body.ToArray()));
            });
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task SubscribeToString<T>(string queueName, Func<string, bool> processReceived)
        {
            var exchangeName = GetRouteName(typeof(T));
            SubscribeCore(exchangeName, queueName, _isAutoDeleteQueue, false, (channel, ea) =>
            {
                if (processReceived(Encoding.UTF8.GetString(ea.Body.ToArray())))
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
                ;
            });
            return Task.CompletedTask;
        }

        #endregion


        #region private methods

        private IModel SubscribeCore(string exchangeName, string queueName, bool isAutoAck, bool isAutoDeleteQueue, Action<IModel, BasicDeliverEventArgs> processReceived)
        {
            var exchangeFullName = $"{_prefix}.{exchangeName}";
            var queueFullName = $"{_prefix}.{exchangeName}.{queueName}";
            if (!_models.TryGetValue(queueFullName, out var channel))
            {
                // define and create MQ
                channel = DeclareChannelAndQueInfo(_connection, queueFullName, isAutoDeleteQueue, exchangeFullName);

                // subsribe
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    processReceived(channel, ea);
                };
                var tag = channel.BasicConsume(queue: queueFullName,
                                     autoAck: isAutoAck,
                                     consumer: consumer
                                     );
                _models.TryAdd(queueFullName, channel);
                _consumeTags.TryAdd(queueFullName, tag);
                return channel;

            }
            throw new Exception($"{queueFullName} has been subscribed");
        }

        /// <summary>
        /// dynamic build Expression (instance, target) => instance./MethodName/.Invoke(Deserialize(target, Convert(null, JsonSerializerOptions)))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// create Queue、Exchange and binding
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="exchangeName"></param>
        /// <returns></returns>
        private static IModel DeclareChannelAndQueInfo(IConnection connection, string queueName, bool isAutoDeleteQueue, string exchangeName)
        {
            // Create Exchange
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, false, false, null);

            // Create Queue
            channel.QueueDeclare(queueName, false, false, isAutoDeleteQueue, null);

            // 把Queue跟Exchange bind
            channel.QueueBind(queueName, exchangeName, "");

            channel.BasicQos(0, 10, false);
            return channel;
        }
        #endregion

        #region dispose

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_models != null)
                {
                    foreach (var channel in _models)
                    {
                        channel.Value?.BasicCancel(_consumeTags[channel.Key]);
                        channel.Value?.Dispose();
                    }
                }
            }
        }

        #endregion
    }
}
