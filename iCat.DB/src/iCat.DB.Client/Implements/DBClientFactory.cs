using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Implements
{
    public class DBClientFactory: IDBClientFactory
    {
        private readonly IConnectionStringProvider _provider;
        private readonly Dictionary<string, DBClient> _dbClients = new Dictionary<string, DBClient>();


        public DBClientFactory(IConnectionStringProvider provider)
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
                        var connectionData = _provider.GetConnectionDatas().First(p => p.Category == category);
                        _dbClients.Add(category, (DBClient)Activator.CreateInstance(connectionData.DBClientType!, category, connectionData.ConnectionString)!);
                    }
                }
            }
            return _dbClients[category];
        }

    }
}
