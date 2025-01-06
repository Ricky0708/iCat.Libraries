using iCat.Authorization.Models;
using iCat.Authorization.Web.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Web.Models
{
    /// <summary>
    /// Authorized base model
    /// </summary>
    /// <typeparam name="PrivilegeEnum"></typeparam>
    public abstract class AuthorizedPrivilege<PrivilegeEnum> where PrivilegeEnum : Enum
    {
        private readonly IPrivilegeProvider<PrivilegeEnum> _privilegeProvider;

        /// <summary>
        /// Authorized base model
        /// </summary>
        /// <param name="privilegeProvider"></param>
        protected AuthorizedPrivilege(IPrivilegeProvider<PrivilegeEnum> privilegeProvider)
        {
            _privilegeProvider = privilegeProvider;
        }

        /// <summary>
        /// Current user privileges
        /// </summary>
        public IEnumerable<Privilege<PrivilegeEnum>> Privileges => _privilegeProvider.GetCurrentUserPrivileges();
    }
}
