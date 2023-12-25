using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.RabbitMQ.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExchangeAttribute : Attribute
    {
        public string Name { get; }
        public ExchangeAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

    }
}
