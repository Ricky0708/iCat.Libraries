using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using iCat.MQ.Abstraction.Abstractions;

namespace iCat.RabbitMQ.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class Publisher : BasePublisher
    {

        #region fields

        private readonly IConnection _connection;
        private readonly string _prefix;
        private readonly ChannelManager _channelManager;
        private delegate void delgExecuter(IModel channel, string exchange, ReadOnlyMemory<byte> body);
        private readonly delgExecuter? _sender;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="category"></param>
        /// <param name="prefix"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Publisher(IConnection connection, string category, string prefix) : base(category)
        {
            _connection = connection;
            _prefix = prefix;
            _connection = connection ?? throw new ArgumentNullException(nameof(_connection));
            _channelManager = new ChannelManager(_connection);

            var channelExpr = Expression.Parameter(typeof(IModel), "channel");
            var exchangeExpr = Expression.Parameter(typeof(string), "exchange");
            var routingKeyExpr = Expression.Constant("");
            var mandatoryExpr = Expression.Constant(false);
            var basicPropertiesExpr = Expression.Convert(Expression.Constant(null), typeof(IBasicProperties));
            var bodyPropertiesExpr = Expression.Parameter(typeof(ReadOnlyMemory<byte>), "body");

            var methodInfo = typeof(IModelExensions).GetMethods().First(p => p.Name == "BasicPublish" && p.GetParameters().Length == 6)!;
            var publishMxpr = Expression.Call(null, methodInfo, channelExpr, exchangeExpr, routingKeyExpr, mandatoryExpr, basicPropertiesExpr, bodyPropertiesExpr);
            var lambda = Expression.Lambda<delgExecuter>(publishMxpr, channelExpr, exchangeExpr, bodyPropertiesExpr);
            _sender = lambda.Compile();
        }

        /// <summary>
        /// <see cref="BasePublisher.SendAsync{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task SendAsync<T>(T data)
        {
            var exchangeName = base.GetRouteName(data);
            await _channelManager.Execute(channel => _sender!.Invoke(channel, $"{_prefix}.{exchangeName}", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data))));
        }
    }
}
