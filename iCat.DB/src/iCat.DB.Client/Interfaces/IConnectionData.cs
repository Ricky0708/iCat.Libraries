using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.DB.Client.Implements;

namespace iCat.DB.Client.Interfaces
{
    public interface IConnectionData<out T> where T : DBClient
    {
        Type? DBClientType { get; }
        string ConnectionString { get; set; }
    }
}
