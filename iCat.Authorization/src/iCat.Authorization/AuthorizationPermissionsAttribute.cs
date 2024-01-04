using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization
{
    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the function enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// [AuthorizationPermissions(UserProfilePermission.Add | UserProfilePermission.Read)]
        /// </summary>
        /// <param name="permission"></param>
        public AuthorizationPermissionsAttribute(params object[] permission)
        {
        }
    }

    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1> : AuthorizeAttribute where T1 : Enum
    {
        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the function enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// [AuthorizationPermissions<UserProfilePermission>(UserProfilePermission.Add | UserProfilePermission.Read)]
        /// </summary>
        /// <param name="permission"></param>
        public AuthorizationPermissionsAttribute(T1 permission)
        {
        }
    }

    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1, T2> : AuthorizationPermissionsAttribute<T1> where T1 : Enum where T2 : Enum
    {
        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the function enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// [AuthorizationPermissions<UserProfilePermission, OrderPermission>(UserProfilePermission.Add | UserProfilePermission.Read, OrderPermission.Add)]
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public AuthorizationPermissionsAttribute(T1 t1, T2 t2) : base(t1)
        {
        }
    }

    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1, T2, T3> : AuthorizationPermissionsAttribute<T1, T2> where T1 : Enum where T2 : Enum where T3 : Enum
    {
        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the function enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// [AuthorizationPermissions<UserProfilePermission, OrderPermission, DepartmentPermission>((UserProfilePermission)int.MaxValue, (OrderPermission)int.MaxValue, (DepartmentPermission)int.MaxValue)]
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        public AuthorizationPermissionsAttribute(T1 t1, T2 t2, T3 t3) : base(t1, t2)
        {
        }
    }

    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1, T2, T3, T4> : AuthorizationPermissionsAttribute<T1, T2, T3> where T1 : Enum where T2 : Enum where T3 : Enum where T4 : Enum
    {
        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the function enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// [AuthorizationPermissions<UserProfilePermission, OrderPermission, DepartmentPermission, PagePermission>((UserProfilePermission)int.MaxValue, (OrderPermission)int.MaxValue, (DepartmentPermission)int.MaxValue, PagePermission.Add)]
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        public AuthorizationPermissionsAttribute(T1 t1, T2 t2, T3 t3, T4 t4) : base(t1, t2, t3)
        {
        }
    }
}
