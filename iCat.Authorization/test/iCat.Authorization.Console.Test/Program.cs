﻿using iCat.Authorization.ConsoleTest.Enum;
using iCat.Authorization.Web;
using System.Reflection;

namespace iCat.Authorization.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //var method = typeof(Program).GetMethod("ss", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            //a.GetAuthorizationPermissionsData(method!.CustomAttributes.ToArray());
            Console.WriteLine("Hello, World!");
        }

        //[AuthorizationPermissions<UserProfilePermission, OrderPermission, UserProfilePermission>(
        //    UserProfilePermission.Add | UserProfilePermission.Read,
        //    OrderPermission.Edit | OrderPermission.Delete,
        //    UserProfilePermission.Edit | UserProfilePermission.Delete)]
        [AuthorizationPermissions(
            UserProfilePermission.Add | UserProfilePermission.Read,
            OrderPermission.Edit | OrderPermission.Delete,
            UserProfilePermission.Edit | UserProfilePermission.Delete)]
        public static void ss()
        {

        }
    }
}
