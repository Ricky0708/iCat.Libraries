using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.Authorization.Models;
using System.Text.Json;
using System.Reflection;
using iCat.Authorization.Utilities;
using iCat.Authorization.Constants;

namespace iCat.Authorization.Utilities.Tests
{
    [TestClass()]
    public class FunctionPermissionParserTests
    {
        [TestMethod()]
        public void GetFunctionPermissionDefinitions_Success()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Function_Success));
            var validationData = new List<Function> {
                new() {
                    Name = "UserProfile",
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = "Order",
                    Value = 2,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 2 },
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = "Department",
                    Value = 3,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = parser.GetDefinitions();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetFunctionPermissionDefinitions_Fail1()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Function_Fail));
            var validationData = new List<Function> {
                new() {
                    Name = "UserProfile",
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = "Department",
                    Value = 3,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = parser.GetDefinitions();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetFunctionPermissionDefinitions_Fail2()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Function_Fail));
            var validationData = new List<Function> {
                new() {
                    Name = "UserProfile",
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = "Department",
                    Value = 3,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = parser.GetDefinitions();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetAuthorizationPermissionsDataTest_Success()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Function_Success));
            var method = typeof(FunctionPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<Function> {
                new() {
                    Name = "UserProfile",
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = "Order",
                    Value = 2,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = parser.GetPermissionRequired(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetAuthorizationPermissionsDataTest_Fail()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Function_Success));
            var method = typeof(FunctionPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<Function> {
                new() {
                    Name = "UserProfile",
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = "Order",
                    Value = 2,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = parser.GetPermissionRequired(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetClaimFromFunctionPermissionDataTest()
        {
            // arrange
            var permissionProvider = new DefaultPermissionProvider(typeof(Function_Success));
            var permitProvider = new DefaultPermitProvider(null, permissionProvider);

            // action
            var result = permitProvider.GeneratePermitClaim(new Function
            {
                Name = "A",
                Value = 1,
                PermissionsData = new List<Permission>
                {
                    new() { Name = "Add", Value = 1}
                }
            });

            // assert
            Assert.AreEqual(result.Type, AuthorizationPermissionClaimTypes.Permit);
            Assert.AreEqual(result.Value, "1,1");
        }


        [AuthorizationPermissions(
        UserProfilePermission.Add | UserProfilePermission.Read,
        OrderPermission.Edit | OrderPermission.Delete,
        UserProfilePermission.Edit | UserProfilePermission.Delete)]
        public static void TestAttributeMethod()
        {

        }


    }


    public enum Function_Success
    {
        [Permission(typeof(UserProfilePermission))]
        UserProfile = 1,
        [Permission(typeof(OrderPermission))]
        Order = 2,
        [Permission(typeof(DepartmentPermission))]
        Department = 3
    }

    public enum Function_Fail
    {
        [Permission(typeof(UserProfilePermission))]
        UserProfile = 1,
        [Permission(typeof(DepartmentPermission))]
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
