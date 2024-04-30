using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Factory.Interfaces
{
    /// <summary>
    /// Connection Factory
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Get Connection by key;
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IConnection GetConnection(string key);

        /// <summary>
        /// Get all Connections
        /// </summary>
        /// <returns></returns>
        IEnumerable<IConnection> GetConnections();
    }
}
