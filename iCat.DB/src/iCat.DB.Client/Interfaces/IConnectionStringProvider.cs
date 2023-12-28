using iCat.DB.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Implements;

namespace iCat.DB.Client.Interfaces
{
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IConnectionData<DBClient> GetConnectionData(string key);
    }
}
