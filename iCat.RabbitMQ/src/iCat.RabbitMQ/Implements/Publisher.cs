using iCat.RabbitMQ.Attributes;
using iCat.RabbitMQ.Interfaces;
using iCat.RabbitMQ.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace iCat.RabbitMQ.Implements
{
    public class Publisher : IPublisher
    {
        #region fields

        private readonly IConnection _connection;
        private readonly string _prefix;
        private delegate void delgExecuter(IModel channel, string exchange, ReadOnlyMemory<byte> body);
        private readonly delgExecuter? _sender;
        private readonly ChannelManager _channelManager;

        #endregion


        public Publisher(IConnection connection, string prefix)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(_connection));
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            _channelManager = new ChannelManager(_connection);

            var channelExpr = Expression.Parameter(typeof(IModel), "channel");
            var exchangeExpr = Expression.Parameter(typeof(string), "exchange");
            var routingKeyExpr = Expression.Constant("");
            var mandatoryExpr = Expression.Constant(false);
            var basicPropertiesExpr = Expression.Convert(Expression.Constant(null), typeof(IBasicProperties));
            var bodyPropertiesExpr = Expression.Parameter(typeof(ReadOnlyMemory<byte>), "body");

            var methodInfo = typeof(IModelExensions).GetMethods().First(p => p.Name == "BasicPublish" && p.GetParameters().Length == 6)!;
            var publishMxpr = Expression.Call(null, methodInfo, channelExpr, exchangeExpr, routingKeyExpr, mandatoryExpr, basicPropertiesExpr, bodyPropertiesExpr);
            //var lambda2 = Expression.Lambda(publishMxpr, channelExpr, exchangeExpr, bodyPropertiesExpr);
            var lambda = Expression.Lambda<delgExecuter>(publishMxpr, channelExpr, exchangeExpr, bodyPropertiesExpr);
            _sender = lambda.Compile();
        }

        ///// <summary>
        ///// <see cref="IPublisher.GetBasicExchangesName"/>
        ///// </summary>
        ///// <returns></returns>
        //public List<string> GetBasicExchangesName()
        //{
        //    var asses = AppDomain.CurrentDomain.GetAssemblies();
        //    var exchangesNames = new List<string>();
        //    foreach (var ass in asses)
        //    {
        //        var types = ass.GetTypes().Where(p => (p.BaseType?.Name ?? "") == typeof(BaseSubscriber<>).Name).ToList();
        //        exchangesNames.AddRange(types.SelectMany(p => p.GetFields().Select(p => _prefix + "." + p.FieldType.GetGenericArguments()[0].CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value)).Distinct().ToList());
        //    }

        //    return exchangesNames;
        //}

        ///// <summary>
        ///// <see cref="IPublisher.GetModelCanBeUsed"/>
        ///// </summary>
        ///// <returns></returns>
        //public List<string> GetModelCanBeUsed()
        //{
        //    var asses = AppDomain.CurrentDomain.GetAssemblies();
        //    var modelNames = new List<string>();
        //    foreach (var ass in asses)
        //    {
        //        var types = ass.GetTypes().Where(p => (p.BaseType?.Name ?? "") == typeof(_baseSubscriber<>).Name).ToList();
        //        modelNames.AddRange(types.SelectMany(p =>
        //        {
        //            return p.GetFields().Select(p =>
        //            {
        //                //return p.FieldType.GetGenericArguments()[0].CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value as string;
        //                return p.FieldType.GetGenericArguments()[0].Name;
        //            });
        //        }).Distinct().ToList());
        //    }

        //    return modelNames;
        //}

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync<T>(T data) where T : BaseMQDataModel
        {
            var exchangeName = default(string);
            var instanceType = data.GetType();
            if (instanceType.IsArray ||
              instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
              instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(List<>))
            {
                exchangeName = data.GetType().GetGenericArguments().First().GetType().CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value as string;
            }
            else
            {
                exchangeName = data.GetType().CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value as string;
            }
            await _channelManager.Execute(channel => _sender!.Invoke(channel, $"{_prefix}.{exchangeName}", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data))));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync<T>(List<T> data) where T : BaseMQDataModel
        {
            var exchangeName = default(string);
            var instanceType = data.GetType();
            if (instanceType.IsArray ||
              instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
              instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(List<>))
            {
                exchangeName = data.GetType().GetGenericArguments().First().CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value as string;
            }
            else
            {
                exchangeName = data.GetType().CustomAttributes.First(p => p.AttributeType == typeof(ExchangeAttribute)).ConstructorArguments[0].Value as string;
            }
            await _channelManager.Execute(channel => _sender!.Invoke(channel, $"{_prefix}.{exchangeName}", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data))));
        }
    }
}
