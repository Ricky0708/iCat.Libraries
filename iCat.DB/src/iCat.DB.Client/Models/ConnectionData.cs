using iCat.DB.Client.Constants;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Models
{
    public class ConnectionData<T> : IConnectionData<T> where T : DBClient
    {
        public Type? DBClientType => typeof(T);
        public string ConnectionString { get; set; } = "";
    }
}
