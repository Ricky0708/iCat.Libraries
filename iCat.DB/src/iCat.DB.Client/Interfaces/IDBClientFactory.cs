using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Interfaces
{
    public interface IDBClientFactory
    {
        IUnitOfWork GetUnitOfWork(string category);
        IConnection GetConnection(string category);
    }
}
