using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iCat.DB.Client.Constants.ExecuteCommand;

namespace iCat.DB.Client.Delegates
{
    /// <summary>
    /// 
    /// </summary>
    public class Handlers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="commandKind"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public delegate void ExectuedCommandHandler2(string? category, CommandKind commandKind, IDbCommand? command, IDbDataParameter[]? parameters);
    }
}
