using iCat.RabbitMQ.Attributes;
using iCat.RabbitMQ.Interfaces;
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

namespace iCat.RabbitMQ.Implements
{
    public class Subscriber : ISubscriber
    {
        private readonly IConnection _connection;
        private readonly string _prefix;

        private delegate T delgExecuter<T>(string message);
        private readonly ConcurrentDictionary<string, IModel> _models;
        private readonly ConcurrentDictionary<string, string> _consumeTags;

        public Subscriber(IConnection connection, string prefix)
        {
            this._connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this._prefix = prefix;
            _models = new ConcurrentDictionary<string, IModel>();
            _consumeTags = new ConcurrentDictionary<string, string>();
        }

        #region public methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        public IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Action<T> processReceived)
        {
            return SubscribeCore<T>(queueName, isAutoDeleteQueue, true, (channel, lambda, ea) =>
            {
                processReceived(lambda.Invoke(Encoding.UTF8.GetString(ea.Body.ToArray())));
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        public IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Func<T, bool> processReceived)
        {
            return SubscribeCore<T>(queueName, isAutoDeleteQueue, false, (channel, lambda, ea) =>
            {
                if (processReceived(lambda.Invoke(Encoding.UTF8.GetString(ea.Body.ToArray()))))
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    channel.BasicNack(ea.DeliveryTag, false, true);
                };
            });
        }

        #endregion

        #region private methods

        private IModel SubscribeCore<T>(string queueName, bool isAutoDeleteQueue, bool isAutoAck, Action<IModel, delgExecuter<T>, BasicDeliverEventArgs> processReceived)
        {
            var exchangeName = GetExchangeName<T>();
            var exchangeFullName = $"{_prefix}.{exchangeName}";
            var queueFullName = $"{_prefix}.{exchangeName}.{queueName}";
            if (!_models.TryGetValue(queueFullName, out var channel))
            {
                // define and create MQ
                channel = DeclareChannelAndQueInfo(_connection, queueFullName, isAutoDeleteQueue, exchangeFullName);

                // create delegate
                var lambda = BuildDelg<T>(typeof(T));

                // subsribe
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    processReceived(channel, lambda, ea);
                };
                var tag = channel.BasicConsume(queue: queueFullName,
                                     autoAck: isAutoAck,
                                     consumer: consumer
                                     );
                _models.TryAdd(queueFullName, channel);
                _consumeTags.TryAdd(queueFullName, tag);
                return channel;

            }
            throw new Exception($"{typeof(T).Name} has been subscribed");
        }

        private static string GetExchangeName<T>()
        {
            var exchangeName = default(string);
            if (typeof(T).IsArray ||
                   typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                   typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
            {
                exchangeName = typeof(T).GetGenericArguments().First().CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value as string;
            }
            else
            {
                exchangeName = typeof(T).CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value as string;
            }
            return exchangeName!;
        }

        /// <summary>
        /// dynamic build Expression (instance, target) => instance./MethodName/.Invoke(Deserialize(target, Convert(null, JsonSerializerOptions)))
        /// </summary>
        /// <param name="exchangeMethod"></param>
        /// <returns></returns>
        private static delgExecuter<T> BuildDelg<T>(Type type)
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
            // 建立Exchange
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, false, false, null);

            // 建立Queue
            channel.QueueDeclare(queueName, false, false, isAutoDeleteQueue, null);

            // 把Queue跟Exchange bind
            channel.QueueBind(queueName, exchangeName, "");

            channel.BasicQos(0, 10, false);
            return channel;
        }
        #endregion

        #region dispose

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
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
