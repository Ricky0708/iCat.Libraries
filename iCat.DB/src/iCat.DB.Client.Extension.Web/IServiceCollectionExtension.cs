using iCat.DB.Client.Factory.Implements;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Factory.Models;
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
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDBClient(this IServiceCollection services, params DBClient[] dbClients)
        {
            foreach (var dbClient in dbClients)
            {
                services.AddScoped<IConnection>(s => dbClient);
            };
            services.AddScoped<IEnumerable<IUnitOfWork>>(s => s.GetServices<IConnection>().Select(p => (IUnitOfWork)p));
            return services;
        }

        public static IServiceCollection AddDBClientFactory(this IServiceCollection services, params Expression<Func<DBClient>>[] dbClients)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddSingleton<IConnectionProvider>(s => new DefaultConnectionProvider(dbClients));
            return services;
        }

        public static IServiceCollection AddDBClientFactory(this IServiceCollection services, IConnectionProvider connectionProvider)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddSingleton<IConnectionProvider>(s => connectionProvider);
            return services;
        }
    }
}
