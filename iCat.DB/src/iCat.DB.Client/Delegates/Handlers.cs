using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iCat.DB.Client.Constants.ExecuteCommand;

namespace iCat.DB.Client.Delegates
{
    public class Handlers
    {
        public delegate void ExectuedCommandHandler(string category, Command command, string script);
    }
}
