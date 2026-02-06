using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Kafka.Models
{
    /// <summary>
    /// Consumer configuration for Kafka, inherits from <see cref="Confluent.Kafka.ConsumerConfig"/>
    /// </summary>
    public class ConsumerConfig : Confluent.Kafka.ConsumerConfig
    {
        /// <summary>
        /// The number of partitions for the topic. This is the number of separate logs that will be maintained for the topic. Each partition can be thought of as a separate queue that can be consumed independently. The default value is 1, which means that there will be only one partition for the topic. You can set this to a higher value if you want to increase the parallelism of your consumers and improve the throughput of your Kafka cluster.
        /// </summary>
        public int Partitions { get; set; }

        /// <summary>
        /// The replication factor for the topic. This is the number of copies of the topic that will be maintained across the Kafka cluster. A higher replication factor provides better fault tolerance, but also requires more resources. The default value is 1, which means that there will be only one copy of the topic. You can set this to a higher value if you want to ensure that your messages are not lost in case of broker failures.
        /// </summary>
        public short ReplicationFactor { get; set; }
    }
}
