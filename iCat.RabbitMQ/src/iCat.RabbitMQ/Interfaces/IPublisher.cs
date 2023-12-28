using iCat.RabbitMQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.RabbitMQ.Interfaces
{
    public interface IPublisher
    {
        ///// <summary>
        ///// Get all available exchanges
        ///// </summary>
        ///// <returns></returns>
        //List<string> GetBasicExchangesName();

        ///// <summary>
        ///// Get all available models
        ///// </summary>
        ///// <returns></returns>
        //List<string> GetModelCanBeUsed();

        /// <summary>
        /// Category
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Send to mq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processId"></param>
        /// <param name="data"></param>
        Task SendAsync<T>(T data) where T : BaseMQDataModel;
    }
}
