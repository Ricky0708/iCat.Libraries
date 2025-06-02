using iCat.MQ.Abstraction.Attributes;
using iCat.MQ.Abstraction.Interfaces;
using iCat.MQ.Abstraction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.MQ.Abstraction.Abstractions
{
    public abstract class BasePublisher : IPublisher
    {
        private readonly string _category;

        /// <summary>
        /// <see cref="IPublisher.Category"/>
        /// </summary>
        public string Category => _category;


        protected BasePublisher(string category)
        {
            _category = category;
        }

        /// <summary>
        /// <see cref="IPublisher.SendAsync{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract Task SendAsync<T>(T data) where T : BaseMQDataModel;

        protected string GetRouteName(object instance)
        {
            var RouteName = default(string);
            var instanceType = instance.GetType();
            if (instanceType.IsArray ||
              instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
              instanceType.IsGenericType && instanceType.GetGenericTypeDefinition() == typeof(List<>))
            {
                RouteName = instance.GetType().GetGenericArguments().First().GetType().CustomAttributes.First(p => p.AttributeType == typeof(MessageRouteAttribute)).ConstructorArguments[0].Value as string;
            }
            else
            {
                RouteName = instance.GetType().CustomAttributes.First(p => p.AttributeType == typeof(MessageRouteAttribute)).ConstructorArguments[0].Value as string;
            }
            return RouteName ?? throw new ArgumentException("Can't get route name from the instance.");

        }
    }
}
