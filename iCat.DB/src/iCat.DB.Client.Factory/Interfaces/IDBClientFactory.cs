using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Factory.Interfaces
{
    /// <summary>
    /// DB Client Factory
    /// </summary>
    [Obsolete("Please use IConnectionFactory, IUnitOfWorkFactory instead.", false)]
    public interface IDBClientFactory : IConnectionFactory, IUnitOfWorkFactory
    {
    }
}
