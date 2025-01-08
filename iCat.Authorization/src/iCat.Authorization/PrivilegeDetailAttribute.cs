using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PrivilegeDetailAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public PrivilegeDetailAttribute(Type permissionEnumTypes)
        {
        }
    }
}
