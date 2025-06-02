using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Abstraction.Interfaces
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
        /// Subscribe a queue/topic, auto ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        Task Subscribe<T>(string messageGroup, Action<T> processReceived);

        /// <summary>
        /// Subscribe a queue/topic, ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        Task Subscribe<T>(string messageGroup, Func<T, bool> processReceived);

        /// <summary>
        /// Subscribe a queue/topic, auto ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        Task SubscribeToString<T>(string messageGroup, Action<string> processReceived);

        /// <summary>
        /// Subscribe a queue/topic, ack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="processReceived"></param>
        /// <returns></returns>
        Task SubscribeToString<T>(string messageGroup, Func<string, bool> processReceived);
    }
}
