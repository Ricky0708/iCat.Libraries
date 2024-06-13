using iCat.DB.Client.Factory.Implements;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Implements;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Factory.Extensions
{
    /// <summary>
    /// extension
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Register the factory with a ServiceProvider using default connection provider
        /// IConnection and IUnitOfWork created by the factory are different from throse created by ServiceProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbClients"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBFactory(this IServiceCollection services, params Expression<Func<DBClient>>[] dbClients)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddScoped<IConnectionFactory>(s => s.GetRequiredService<IDBClientFactory>());
            services.AddScoped<IUnitOfWorkFactory>(s => s.GetRequiredService<IDBClientFactory>());
            services.AddSingleton<IDBClientProvider>(s => new DefaultDBClientProvider(dbClients));
            return services;

        }

        /// <summary>
        /// Register the factory with a ServiceProvider using default connection provider
        /// IConnection and IUnitOfWork created by the factory are different from throse created by ServiceProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbClients"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBFactory(this IServiceCollection services, Func<IServiceProvider, Expression<Func<DBClient>>[]> dbClients)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddScoped<IConnectionFactory>(s => s.GetRequiredService<IDBClientFactory>());
            services.AddScoped<IUnitOfWorkFactory>(s => s.GetRequiredService<IDBClientFactory>());
            services.AddSingleton<IDBClientProvider>(s => new DefaultDBClientProvider(dbClients.Invoke(s)));
            return services;

        }

        /// <summary>
        /// Register the factory with a ServiceProvider using custom connection provider
        /// IConnection and IUnitOfWork created by the factory are different from throse created by ServiceProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="implementationDBClientProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBFactory(this IServiceCollection services, Func<IServiceProvider, IDBClientProvider> implementationDBClientProvider)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddScoped<IConnectionFactory>(s => s.GetRequiredService<IDBClientFactory>());
            services.AddScoped<IUnitOfWorkFactory>(s => s.GetRequiredService<IDBClientFactory>());
            services.AddSingleton<IDBClientProvider>(implementationDBClientProvider);
            return services;

        }
    }
}
