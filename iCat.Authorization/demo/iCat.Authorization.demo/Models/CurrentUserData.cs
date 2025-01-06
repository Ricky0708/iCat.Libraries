using iCat.Authorization.demo.Enums;
using iCat.Authorization.Web.Models;
using iCat.Authorization.Web.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.demo.Models
{
    public class CurrentUserData : AuthorizedPrivilege<PrivilegeEnum>
    {
        public CurrentUserData(IPrivilegeProvider<PrivilegeEnum> privilegeProvider) : base(privilegeProvider)
        {
        }
    }
}
