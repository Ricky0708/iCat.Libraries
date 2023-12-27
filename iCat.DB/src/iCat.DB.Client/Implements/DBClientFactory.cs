using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Implements
{
    public class DBClientFactory : IDBClientFactory
    {
        private readonly IConnectionStringProvider _provider;
        private readonly Dictionary<string, DBClient> _dbClients = new Dictionary<string, DBClient>();


        public DBClientFactory(IConnectionStringProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IUnitOfWork GetUnitOfWork(string key)
        {
            return (IUnitOfWork)GetInstance(key);

        }

        public IConnection GetConnection(string key)
        {
            return (IConnection)GetInstance(key);
        }

        private DBClient GetInstance(string key)
        {
            if (!_dbClients.TryGetValue(key, out var result))
            {
                lock (_dbClients)
                {
                    if (!_dbClients.TryGetValue(key, out result))
                    {
                        var connectionData = _provider.GetConnectionDatas()[key];
                        _dbClients.Add(key, (DBClient)Activator.CreateInstance(connectionData.DBClientType!, key, connectionData.ConnectionString)!);
                    }
                }
            }
            return _dbClients[key];
        }

    }
}
