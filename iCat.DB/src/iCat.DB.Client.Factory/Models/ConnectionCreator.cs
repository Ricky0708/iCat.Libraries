using iCat.DB.Client.Constants;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace iCat.DB.Client.Factory.Models
{
    public class ConnectionCreator
    {
        public Expression<Func<DBClient>>? ConnectionGenerator { get; set; }
    }
}
