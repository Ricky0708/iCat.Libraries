using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Implements;
namespace iCat.DB.Client.Factory.Implements
{
    public class DBClientFactory : IDBClientFactory
    {
        private readonly IConnectionProvider _provider;
        private readonly Dictionary<string, DBClient> _dbClients = new Dictionary<string, DBClient>();


        public DBClientFactory(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IUnitOfWork GetUnitOfWork(string category)
        {
            return (IUnitOfWork)GetInstance(category);

        }

        public IConnection GetConnection(string category)
        {
            return (IConnection)GetInstance(category);
        }

        private DBClient GetInstance(string category)
        {
            if (!_dbClients.TryGetValue(category, out var result))
            {
                lock (_dbClients)
                {
                    if (!_dbClients.TryGetValue(category, out result))
                    {
                        var func = _provider.GetConnectionData(category);
                        var dbClient = func?.Invoke() ?? throw new NotImplementedException();
                        dbClient.DisposingHandler += RemoveInstance;
                        _dbClients.Add(category, dbClient);
                    }
                }
            }
            return _dbClients[category];
        }

        private void RemoveInstance(object? sender, EventArgs e)
        {
            var key = ((DBClient?)sender)!.Category;
            if (_dbClients.ContainsKey(key))
            {
                lock (_dbClients)
                {
                    if (_dbClients.ContainsKey(key))
                    {
                        _dbClients.Remove(key);
                    }
                }
            }
        }

    }
}
