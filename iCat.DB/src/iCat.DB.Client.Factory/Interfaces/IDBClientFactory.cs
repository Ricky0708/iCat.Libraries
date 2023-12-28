using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Factory.Interfaces
{
    public interface IDBClientFactory
    {
        IUnitOfWork GetUnitOfWork(string key);
        IConnection GetConnection(string key);
    }
}
