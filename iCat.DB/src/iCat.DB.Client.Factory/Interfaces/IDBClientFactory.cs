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
    internal interface IDBClientFactory : IConnectionFactory, IUnitOfWorkFactory
    {
    }
}
