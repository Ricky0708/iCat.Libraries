using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Abstraction.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MessageRouteAttribute : Attribute
    {
        public string Name { get; }

        /// <summary>
        /// Create/Bind Name of exchange/topic in RabbitMQ/Kafka.
        /// </summary>
        /// <param name="name">exchange/topic name</param>
        /// <exception cref="ArgumentNullException"></exception>
        public MessageRouteAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
