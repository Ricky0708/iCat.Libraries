using System;
using System.Collections.Generic;
using System.Data.Common;
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
        public string Category { get; init; }

        /// <summary>
        /// Connection string
        /// </summary>
        public DbConnection Connection { get; init; }

        /// <summary>
        /// DBClient info
        /// </summary>
        /// <param name="category"></param>
        /// <param name="connection"></param>
        public DBClientInfo(string category, DbConnection connection)
        {
            Category = category;
            Connection = connection;
        }
    }
}
