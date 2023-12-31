﻿using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Factory.Models;
using System.Linq.Expressions;
using iCat.DB.Client.Models;

namespace iCat.DB.Client.Factory.Implements
{
    /// <summary>
    /// Connection info provider
    /// </summary>
    public class DefaultDBClientProvider : IDBClientProvider
    {
        private readonly Dictionary<string, Func<DBClient>> _connectionDatas;

        /// <summary>
        /// Connection info provider
        /// </summary>
        /// <param name="dbClients"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultDBClientProvider(params Expression<Func<DBClient>>[] dbClients)
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
            };

            _connectionDatas = dbClients?.ToDictionary(
                p => GetCategory(p),
                p => p?.Compile() ?? throw new ArgumentException(nameof(p)))
                ?? throw new ArgumentException(nameof(dbClients));
        }

        /// <summary>
        /// Get db client
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public Func<DBClient> GetDBClientCreator(string category)
        {
            return _connectionDatas[category];
        }

        private string GetCategory(Expression<Func<DBClient>>? func)
        {
            var newExpr = func?.Body as NewExpression ?? throw new ArgumentException("");
            if (newExpr != null)
            {
                foreach (var arg in newExpr.Arguments)
                {
                    if (arg is ConstantExpression)
                    {
                        return (arg as ConstantExpression)?.Value?.ToString() ?? "default";
                    }
                    else if (arg is NewExpression)
                    {
                        newExpr = arg as NewExpression;
                        if (newExpr != null)
                        {
                            var value = (newExpr.Arguments[0] as ConstantExpression)?.Value?.ToString() ?? throw new ArgumentException("");
                            return value;
                        }

                    }

                }
            }
            throw new ArgumentException("");
        }
    }
}
