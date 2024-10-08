﻿using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Implements;
namespace iCat.DB.Client.Factory.Implements
{
    /// <summary>
    /// DBClient factory
    /// </summary>
    public class DBClientFactory : IDBClientFactory
    {
        private readonly IDBClientProvider _provider;
        private readonly Dictionary<string, DBClient> _dbClients = new Dictionary<string, DBClient>();
        private bool _disposed = false;

        /// <summary>
        /// DBClient factory
        /// </summary>
        /// <param name="provider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DBClientFactory(IDBClientProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IUnitOfWork GetUnitOfWork(string category)
        {
            return (IUnitOfWork)GetInstance(category);

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IUnitOfWork> GetUnitOfWorks()
        {
            return _dbClients.Select(p => p.Value);

        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IConnection GetConnection(string category)
        {
            return (IConnection)GetInstance(category);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IConnection> GetConnections()
        {
            return _dbClients.Select(p => p.Value);

        }

        private DBClient GetInstance(string category)
        {
            if (!_dbClients.TryGetValue(category, out var result))
            {
                lock (_dbClients)
                {
                    if (!_dbClients.TryGetValue(category, out result))
                    {
                        var func = _provider.GetDBClientCreator(category);
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

            // CA1853
            _dbClients.Remove(key!);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }


        /// <inheritdoc/>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var client in _dbClients) if (client.Value.Connection.State != System.Data.ConnectionState.Closed) client.Value.Close();
            }

            _disposed = true;
        }
    }
}
