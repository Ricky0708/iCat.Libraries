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
        private readonly IEnumerable<ConnectionData> _connectionDatas;

        public DefaultConnectionStringProvider(IEnumerable<ConnectionData> connectionDatas)
        {
            _connectionDatas = connectionDatas ?? throw new ArgumentNullException(nameof(connectionDatas));
        }
        public IEnumerable<ConnectionData> GetConnectionDatas()
        {
            return _connectionDatas;
        }
    }
}
