using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.RabbitMQ.Interfaces
{
    public interface ISubscriber : IDisposable
    {
        /// <summary>
        /// 订阅 mq，自动交易
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">伫列名称，queueName与其他订阅重覆，讯息将会循序接收，否则为广播，所有不同queue name的将会同时收到讯息</param>
        /// <param name="isAutoDeleteQueue">伫列不再被订阅是否自动删除</param>
        /// <param name="processReceived">接收讯息的处理</param>
        /// <returns></returns>
        IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Action<T> processReceived);

        /// <summary>
        /// 订阅 mq，不自动交易
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName">伫列名称，queueName与其他订阅重覆，讯息将会循序接收，否则为广播，所有不同queue name的将会同时收到讯息</param>
        /// <param name="isAutoDeleteQueue">伫列不再被订阅是否自动删除</param>
        /// <param name="processReceived">接收讯息的处理</param>
        /// <returns></returns>
        IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Func<T, bool> processReceived);
    }
}
