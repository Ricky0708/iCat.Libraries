using iCat.DB.Client.Interfaces;
using iCat.DB.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Implements
{
    public class DefaultConnectionStringProvider : IConnectionStringProvider
    {
        private readonly Dictionary<string, ConnectionData> _connectionDatas;

        public DefaultConnectionStringProvider(Dictionary<string, ConnectionData> connectionDatas)
        {
            _connectionDatas = connectionDatas ?? throw new ArgumentNullException(nameof(connectionDatas));
        }
        public Dictionary<string, ConnectionData> GetConnectionDatas()
        {
            return _connectionDatas;
        }
    }
}
