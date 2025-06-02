using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Abstraction.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseMQDataModel
    {
        /// <summary>
        /// Trace Id
        /// </summary>
        public string TraceId { get; set; } = "";

        /// <summary>
        /// Key for the message, can be used for identification or partition of Kafka.
        /// </summary>
        public string? Key { get; set; } = null;
    }
}
