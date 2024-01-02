using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Constants
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteCommand
    {
        /// <summary>
        /// executed command type
        /// </summary>
        public enum CommandKind
        {
            /// <summary>
            /// 
            /// </summary>
            Opened,
            /// <summary>
            /// 
            /// </summary>
            Closed,
            /// <summary>
            /// 
            /// </summary>
            TransactionBegined,
            /// <summary>
            /// 
            /// </summary>
            Commited,
            /// <summary>
            /// 
            /// </summary>
            Rollbacked,
            /// <summary>
            /// 
            /// </summary>
            Executing,
        }
    }
}
