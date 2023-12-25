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
        /// Subscribe a queue, auto ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Action<T> processReceived);

        /// <summary>
        /// Subscribe a queue, auto ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="isAutoDeleteQueue"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        IModel Subscribe<T>(string queueName, bool isAutoDeleteQueue, Func<T, bool> processReceived);
    }
}
