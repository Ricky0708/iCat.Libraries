using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Constants
{
    public class ExecuteCommand
    {
        /// <summary>
        /// executed command type
        /// </summary>
        public enum Command
        {
            Opened,
            Closed,
            TransactionBegined,
            Commited,
            Rollbacked,
            Executed,
        }
    }
}
