using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Factory.Interfaces
{
    /// <summary>
    /// Unit Of Work Factory
    /// </summary>
    public interface IUnitOfWorkFactory : IDisposable
    {
        /// <summary>
        /// Get UnitOfWork by key;
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IUnitOfWork GetUnitOfWork(string key);

        /// <summary>
        /// Get all UnitOfWorks
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnitOfWork> GetUnitOfWorks();
    }
}
