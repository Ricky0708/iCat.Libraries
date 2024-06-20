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
    public class PermissionRelationAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public PermissionRelationAttribute(Type permissionEnumTypes)
        {
        }
    }
}
