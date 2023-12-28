using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Models
{
    public struct DBClientInfo
    {
        public string Category { get; }

        public DBClientInfo(string category)
        {
            Category = category;
        }
    }
}
