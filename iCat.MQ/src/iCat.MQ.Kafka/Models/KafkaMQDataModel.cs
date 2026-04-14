using iCat.MQ.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Kafka.Models
{
    /// <summary>
    /// KafkaMQDataModel is a data model for Kafka messages, it inherits from <see cref="BaseMQDataModel"/> and can be extended with additional properties if needed.
    /// </summary>
    public class KafkaMQDataModel : BaseMQDataModel
    {
        /// <summary>
        /// Key for the message, can be used for identification or partition of Kafka.
        /// </summary>
        public string? Key { get; set; } = null;
    }
}
