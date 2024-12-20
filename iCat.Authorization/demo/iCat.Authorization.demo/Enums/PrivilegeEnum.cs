using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.Authorization;

namespace iCat.Authorization.demo.Enums
{
    public enum PrivilegeEnum
    {
        [PrivilegDetail(typeof(UserProfilePermission))]
        UserProfile = 1,
        [PrivilegDetail(typeof(OrderPermission))]
        Order = 2,
        [PrivilegDetail(typeof(DepartmentPermission))]
        Department = 3
    }

    [Flags]
    public enum UserProfilePermission
    {
        Add = 1,
        Edit = 2,
        ReadPartialDetail = 4,
        Delete = 8,
        ReadAllDetail = 16,
    }

    [Flags]
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
