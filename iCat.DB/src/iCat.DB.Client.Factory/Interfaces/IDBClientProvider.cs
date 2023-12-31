﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Factory.Models;
using iCat.DB.Client.Implements;

namespace iCat.DB.Client.Factory.Interfaces
{
    /// <summary>
    /// Provide DBClients
    /// </summary>
    public interface IDBClientProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Func<DBClient> GetDBClientCreator(string key);
    }
}
