using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Models
{
    /// <summary>
    /// DBClient info
    /// </summary>
    public readonly struct DBClientInfo
    {
        /// <summary>
        /// Category of DBClient
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// DBClient info
        /// </summary>
        /// <param name="category"></param>
        public DBClientInfo(string category)
        {
            Category = category;
        }
    }
}
