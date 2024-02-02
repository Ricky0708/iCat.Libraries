using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.RabbitMQ.Interfaces
{
    /// <summary>
    /// Subscriber
    /// </summary>
    public interface ISubscriber : IDisposable
    {
        /// <summary>
        /// Category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Subscribe a queue, auto ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Action<T> processReceived);

        /// <summary>
        /// Subscribe a queue, ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Func<T, bool> processReceived);

        /// <summary>
        /// Subscribe a queue, auto ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        IModel SubscribeToString<T>(string queueName, bool isAutoDeleteQueue, Action<string> processReceived);

        /// <summary>
        /// Subscribe a queue, ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        IModel SubscribeToString<T>(string queueName, bool isAutoDeleteQueue, Func<string, bool> processReceived);
    }
}
