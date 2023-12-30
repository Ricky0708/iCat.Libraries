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
        public string Category { get; init; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; init; }

        /// <summary>
        /// Do not use
        /// </summary>
        [Obsolete("Use DBClientInfo(string category, string connectionString) for init this", true)]
        public DBClientInfo()
        {
            Category = "";
            ConnectionString = "";
        }

        /// <summary>
        /// DBClient info
        /// </summary>
        /// <param name="category"></param>
        /// <param name="connectionString"></param>
        public DBClientInfo(string category, string connectionString)
        {
            Category = category;
            ConnectionString = connectionString;
        }
    }
}
