using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    public interface IUserPermissionProvider
    {
        IEnumerable<FunctionData> GetUserPermission();
        bool Validate(IEnumerable<FunctionData> ownPermissions, FunctionData permissionRequired);
    }
}
