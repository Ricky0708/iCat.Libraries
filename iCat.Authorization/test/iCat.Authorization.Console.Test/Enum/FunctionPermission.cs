using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.ConsoleTest.Enum
{
    public enum Privilege
    {
        [PrivilegeDetail(typeof(UserProfilePermission))]
        UserProfile = 1,
        [PrivilegeDetail(typeof(OrderPermission))]
        Order = 2,
        [PrivilegeDetail(typeof(DepartmentPermission))]
        Department = 3
    }

    [Flags]
    public enum UserProfilePermission
    {
        Add = 1,
        Edit = 2,
        Read = 4,
        Delete = 8
    }

    public enum OrderPermission
    {
        Add = 1,
        Read = 2,
        Edit = 4,
        Delete = 8
    }

    [Flags]
    public enum DepartmentPermission
    {
        Add = 1,
        Edit = 2,
        Read = 4,
        Delete = 8
    }
}
