using iCat.DB.Client.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Models
{
    public class ConnectionData
    {
        public Type? DBClientType { get; set; }
        public string ConnectionString { get; set; } = "";
    }
}
