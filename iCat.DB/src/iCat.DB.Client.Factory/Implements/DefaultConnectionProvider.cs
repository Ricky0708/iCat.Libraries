using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Factory.Models;
using System.Linq.Expressions;

namespace iCat.DB.Client.Factory.Implements
{
    /// <summary>
    /// Connection info provider
    /// </summary>
    public class DefaultConnectionProvider : IDBClientProvider
    {
        private readonly Dictionary<string, Func<DBClient>> _connectionDatas;

        /// <summary>
        /// Connection info provider
        /// </summary>
        /// <param name="connectionDatas"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultConnectionProvider(params Expression<Func<DBClient>>[] connectionDatas)
        {
            _connectionDatas = connectionDatas?.ToDictionary(
                p => GetCategory(p),
                p => p?.Compile() ?? throw new ArgumentNullException(nameof(p)))
                ?? throw new ArgumentNullException(nameof(connectionDatas));
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
                    newExpr = arg as NewExpression;
                    if (newExpr != null)
                    {
                        var value = (newExpr.Arguments[0] as ConstantExpression)?.Value?.ToString() ?? throw new ArgumentException("");
                        return value;
                    }
                }
            }
            throw new ArgumentException("");
        }
    }
}
