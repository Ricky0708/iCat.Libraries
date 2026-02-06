using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Kafka.Models
{
    internal class ConsumerInfo<TKey>
    {
        public IConsumer<TKey, string> Consumer { get; set; } = default!;
        public ConcurrentDictionary<string, Delegate> Topics { get; set; } = default!;
    }
    internal class TopicInfo
    {
        public Delegate Delegate { get; set; } = default!;
    }
}
