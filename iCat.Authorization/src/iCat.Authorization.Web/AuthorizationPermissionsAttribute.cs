using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Web
{
    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Permission
        /// </summary>
        public object[] Permission { get; }

        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the permit enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// [AuthorizationPermissions(UserProfilePermission.Add | UserProfilePermission.Read)]
        /// </summary>
        /// <param name="permissions"></param>
        public AuthorizationPermissionsAttribute(params object[] permissions)
        {
            Permission = permissions;
        }

    }


#if !NET6_0  
    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1> : AuthorizeAttribute where T1 : Enum
    {
        /// <summary>
        /// Permission
        /// </summary>
        public T1 Permission { get; }

        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the permit enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// <![CDATA[[AuthorizationPermissions<UserProfilePermission>(UserProfilePermission.Add | UserProfilePermission.Read)]]]>
        /// </summary>
        /// <param name="permission"></param>
        public AuthorizationPermissionsAttribute(T1 permission)
        {
            Permission = permission;
        }

    }

    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1, T2> : AuthorizationPermissionsAttribute<T1> where T1 : Enum where T2 : Enum
    {

        /// <summary>
        /// Permission
        /// </summary>
        public T2 Permission2 { get; }

        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the permit enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// <![CDATA[[AuthorizationPermissions<UserProfilePermission, OrderPermission>(UserProfilePermission.Add | UserProfilePermission.Read, OrderPermission.Add)]]]>
        /// </summary>
        /// <param name="permission1"></param>
        /// <param name="permission2"></param>
        public AuthorizationPermissionsAttribute(T1 permission1, T2 permission2) : base(permission1)
        {
            Permission2 = permission2;
        }

    }

    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1, T2, T3> : AuthorizationPermissionsAttribute<T1, T2> where T1 : Enum where T2 : Enum where T3 : Enum
    {

        /// <summary>
        /// Permission
        /// </summary>
        public T3 Permission3 { get; }

        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the permit enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// <![CDATA[[AuthorizationPermissions<UserProfilePermission, OrderPermission, DepartmentPermission>((UserProfilePermission)int.MaxValue, (OrderPermission)int.MaxValue, (DepartmentPermission)int.MaxValue)]]]>
        /// </summary>
        /// <param name="permission1"></param>
        /// <param name="permission2"></param>
        /// <param name="permission3"></param>
        public AuthorizationPermissionsAttribute(T1 permission1, T2 permission2, T3 permission3) : base(permission1, permission2)
        {
            Permission3 = permission3;
        }
    }

    /// <summary>
    /// iCat authorization attribute, 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AuthorizationPermissionsAttribute<T1, T2, T3, T4> : AuthorizationPermissionsAttribute<T1, T2, T3> where T1 : Enum where T2 : Enum where T3 : Enum where T4 : Enum
    {

        /// <summary>
        /// Permission
        /// </summary>
        public T4 Permission4 { get; }

        /// <summary>
        /// Fill in the permission enumeration in the constructor.
        /// The permission enum name needs to match the permit enum name and end with "Permission"
        /// example: UserProfile : UserProfilePermission
        /// <![CDATA[[AuthorizationPermissions<UserProfilePermission, OrderPermission, DepartmentPermission, PagePermission>((UserProfilePermission)int.MaxValue, (OrderPermission)int.MaxValue, (DepartmentPermission)int.MaxValue, PagePermission.Add)]]]>
        /// </summary>
        /// <param name="permission1"></param>
        /// <param name="permission2"></param>
        /// <param name="permission3"></param>
        /// <param name="permission4"></param>
        public AuthorizationPermissionsAttribute(T1 permission1, T2 permission2, T3 permission3, T4 permission4) : base(permission1, permission2, permission3)
        {
            Permission4 = permission4;
        }
    }
#endif
}
