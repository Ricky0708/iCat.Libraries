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
    /// <typeparam name="TPrivilegeEnum"></typeparam>
    public abstract class AuthorizedPrivilege<TPrivilegeEnum> where TPrivilegeEnum : Enum
    {
        private readonly IPrivilegeProvider<TPrivilegeEnum> _privilegeProvider;

        /// <summary>
        /// Authorized base model
        /// </summary>
        /// <param name="privilegeProvider"></param>
        protected AuthorizedPrivilege(IPrivilegeProvider<TPrivilegeEnum> privilegeProvider)
        {
            _privilegeProvider = privilegeProvider;
        }

        /// <summary>
        /// Current user privileges
        /// </summary>
        public IEnumerable<Privilege<TPrivilegeEnum>> Privileges => _privilegeProvider.GetCurrentUserPrivileges();
    }
}
