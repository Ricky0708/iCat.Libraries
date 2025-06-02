using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.RabbitMQ.Models
{
    internal class StackChannelModel
    {
        /// <summary>
        /// last use time
        /// </summary>
        internal long LastUseTime { get; set; }

        /// <summary>
        /// Channel
        /// </summary>
        internal IModel Channel { get; set; } = default!;
    }
}
