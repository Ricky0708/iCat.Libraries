using Confluent.Kafka.Admin;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Kafka.Implements
{
    /// <summary>
    /// Common helper methods for Kafka MQ
    /// </summary>
    internal static class Common
    {
        /// <summary>
        /// Ensure the topic exists, create it if not.
        /// </summary>
        /// <param name="bootstrapServers"></param>
        /// <param name="topic"></param>
        /// <param name="partitions"></param>
        /// <param name="replicationFactor"></param>
        /// <returns></returns>
        internal static async Task<bool> EnsureTopicAsync(
            string bootstrapServers,
            string topic,
            int partitions,
            short replicationFactor)

        {
            var config = new AdminClientConfig
            {
                BootstrapServers = bootstrapServers
            };

            using var admin = new AdminClientBuilder(config).Build();

            try
            {
                await admin.CreateTopicsAsync(new[] {
                    new TopicSpecification
                    {
                        Name = topic,
                        NumPartitions = partitions,
                        ReplicationFactor = replicationFactor
                    }
                });
            }
            catch (CreateTopicsException ex) when (ex.Results.All(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
            {
                // topic exists, ignore it
            }
            return true;
        }
    }
}
