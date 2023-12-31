using iCat.DB.Client.Factory.Implements;
using iCat.DB.Client.Factory.Interfaces;
//using iCat.DB.Client.Factory.Models;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Extension.Web
{
    /// <summary>
    /// extension
    /// </summary>
    public static class IServiceCollectionExtension
    {

        /// <summary>
        /// Register IConnection and IUnitOfWork in ServiceProvider
        /// Instances of them are different from those created by factory
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbClients"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBClient(this IServiceCollection services, params DBClient[] dbClients)
        {
            foreach (var dbClient in dbClients)
            {
                services.AddScoped<IConnection>(s => dbClient);
            };
            services.AddScoped<IEnumerable<IUnitOfWork>>(s => s.GetServices<IConnection>().Select(p => (IUnitOfWork)p));
            return services;
        }

        /// <summary>
        /// Register the factory with a ServiceProvider using default connection provider
        /// IConnection and IUnitOfWork created by the factory are different from throse created by ServiceProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbClients"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBClientFactory(this IServiceCollection services, params Expression<Func<DBClient>>[] dbClients)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddSingleton<IDBClientProvider>(s => new DefaultDBClientProvider(dbClients));
            return services;
        }

        /// <summary>
        /// Register the factory with a ServiceProvider using custom connection provider
        /// IConnection and IUnitOfWork created by the factory are different from throse created by ServiceProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="implementationDBClientProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBClientFactory<TService>(this IServiceCollection services, Func<IServiceProvider, IDBClientProvider> implementationDBClientProvider) where TService : IDBClientProvider
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddSingleton<IDBClientProvider>(implementationDBClientProvider);
            return services;
        }
    }
}
