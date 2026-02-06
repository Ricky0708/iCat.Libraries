using iCat.MQ.Abstraction.Attributes;
using iCat.MQ.Abstraction.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Abstraction.Abstractions
{
    /// <summary>
    /// Base class for subscribers, implements <see cref="ISubscriber"/>
    /// </summary>
    public abstract class BaseSubscriber : ISubscriber
    {
        /// <summary>
        /// <see cref="ISubscriber.Category"/>
        /// </summary>
        public string Category => _category;

        private readonly string _category;
        protected readonly CancellationToken _cancellationToken;

        protected BaseSubscriber(string category, CancellationToken cancellationToken)
        {
            _category = category;
            _cancellationToken = cancellationToken;
        }

        public abstract Task Subscribe<T>(string messageGroup, Action<T> processReceived);

        public abstract Task Subscribe<T>(string messageGroup, Func<T, bool> processReceived);

        public abstract Task SubscribeToString<T>(string messageGroup, Action<string> processReceived);

        public abstract Task SubscribeToString<T>(string messageGroup, Func<string, bool> processReceived);

        protected string GetRouteName(Type type)
        {
            var exchangeName = default(string);
            if (type.IsArray ||
                   type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                   type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                exchangeName = type.GetGenericArguments().First().CustomAttributes.First(p => p.AttributeType == typeof(MessageRouteAttribute)).ConstructorArguments[0].Value as string;
            }
            else
            {
                exchangeName = type.CustomAttributes.First(p => p.AttributeType == typeof(MessageRouteAttribute)).ConstructorArguments[0].Value as string;
            }
            return exchangeName!;
        }

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
