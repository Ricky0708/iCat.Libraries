﻿using iCat.DB.Client.Implements;
using iCat.DB.Client.Interfaces;
using iCat.DB.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Extensions
{
    /// <summary>
    /// Extension
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Register IConnection and IUnitOfWork in ServiceProvider
        /// Instances of them are different from those created by factory
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbClient"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBClient(this IServiceCollection services, Func<IServiceProvider, DBClient> dbClient)
        {
            services.TryAddScoped<IConnection>(dbClient);
            services.TryAddScoped<IUnitOfWork>(s => (IUnitOfWork)s.GetRequiredService<IConnection>());
            return services;
        }

        /// <summary>
        /// Register IConnection and IUnitOfWork in ServiceProvider
        /// Instances of them are different from those created by factory
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbClients"></param>
        /// <returns></returns>
        public static IServiceCollection AddDBClients(this IServiceCollection services, params Expression<Func<IServiceProvider, DBClient>>[] dbClients)
        {
            foreach (var dbClient in dbClients)
            {
                var newExpr = dbClient.Body as NewExpression;
                if (newExpr == null) throw new ArgumentException(nameof(DBClient));
                var hasCategory = false;
                var hasDBClientInfo = false;
                foreach (var arg in newExpr.Arguments)
                {
                    if (arg.Type == typeof(DBClientInfo)) hasDBClientInfo = true;
                    else if (arg.Type == typeof(string)) hasCategory = true;
                }
                if (!(hasDBClientInfo || hasCategory)) throw new ArgumentException($"{nameof(DBClient)} must have category");

                services.AddScoped<IConnection>(dbClient.Compile());
            };
            services.AddScoped<IEnumerable<IUnitOfWork>>(s => s.GetServices<IConnection>().Select(p => (IUnitOfWork)p));
            return services;
        }

    }
}
