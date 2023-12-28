﻿using iCat.DB.Client.Factory.Implements;
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
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Extension.Web
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDBClientFactory(this IServiceCollection services, List<ConnectionCreator> connectionDatas)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddSingleton<IConnectionStringProvider>(s => new DefaultConnectionStringProvider(connectionDatas));
            return services;
        }

        public static IServiceCollection AddDBClientFactory(this IServiceCollection services, IConnectionStringProvider connectionStringProvider)
        {
            services.AddScoped<IDBClientFactory, DBClientFactory>();
            services.AddSingleton<IConnectionStringProvider>(s => connectionStringProvider);
            return services;
        }
    }
}
