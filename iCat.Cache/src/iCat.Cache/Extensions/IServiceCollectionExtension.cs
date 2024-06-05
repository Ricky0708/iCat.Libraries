using iCat.Cache.Implements;
using iCat.Cache.Interfaces;
//using iCat.DB.Client.Factory.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Cache.Extensions
{
    /// <summary>
    /// extension
    /// </summary>
    public static class IServiceCollectionExtension
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddiCatRedisCaching(this IServiceCollection services, ConfigurationOptions config)
        {
            services.AddSingleton<ICache, RedisCacheImpl>(p =>
            {
                var connection = ConnectionMultiplexer.ConnectAsync(config).Result;

                connection.ConnectionFailed += (_, _) =>
                    throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Connect to Redis fail");

                return new RedisCacheImpl(p.GetRequiredService<IDistributedCache>(), connection);
            });
            services.AddStackExchangeRedisCache(o =>
            {
                o.ConnectionMultiplexerFactory = async () =>
                {
                    config.AbortOnConnectFail = false;
                    config.SetDefaultPorts();

                    var connection = await ConnectionMultiplexer.ConnectAsync(config);

                    connection.ConnectionFailed += (_, _) =>
                        throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Connect to Redis fail");

                    return connection;
                };
            });
            return services;
        }
    }
}
