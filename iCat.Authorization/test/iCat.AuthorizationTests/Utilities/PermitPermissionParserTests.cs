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
    public class PermitPermissionParserTests
    {
        [TestMethod()]
        public void GetPermitPermissionDefinitions_Success()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Permit_Success));
            var validationData = new List<Permit> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
                    Value = 2,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 2 },
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
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
        public void GetPermitPermissionDefinitions_Fail1()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Permit_Fail));
            var validationData = new List<Permit> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
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
        public void GetPermitPermissionDefinitions_Fail2()
        {
            // arrange
            var provider = new DefaultPermissionProvider(typeof(Permit_Fail));
            var validationData = new List<Permit> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
                    Value = 3,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = provider.GetDefinitions();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetAuthorizationPermissionsDataTest_Success()
        {
            // arrange
            var parser = new DefaultPermissionProvider(typeof(Permit_Success));
            var method = typeof(PermitPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<Permit> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
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
            var parser = new DefaultPermissionProvider(typeof(Permit_Success));
            var method = typeof(PermitPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<Permit> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<Permission> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
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
        public void GetClaimFromPermitDataTest()
        {
            // arrange
            var permissionProvider = new DefaultPermissionProvider(typeof(Permit_Success));
            var permitProvider = new DefaultPermitProvider(null, permissionProvider);

            // action
            var result = permitProvider.GeneratePermitClaim(new Permit
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
        UserProfileA.Add | UserProfileA.Read,
        OrderB.Edit | OrderB.Delete,
        UserProfileA.Edit | UserProfileA.Delete)]
        public static void TestAttributeMethod()
        {

        }


    }


    public enum Permit_Success
    {
        [Permission(typeof(UserProfileA))]
        UserProfile = 1,
        [Permission(typeof(OrderB))]
        Order = 2,
        [Permission(typeof(DepartmentC))]
        Department = 3
    }

    public enum Permit_Fail
    {
        [Permission(typeof(UserProfileA))]
        UserProfile = 1,
        [Permission(typeof(DepartmentC))]
        Department = 3
    }

    [Flags]
    public enum UserProfileA
    {
        Add = 1,
        Edit = 2,
        Read = 4,
        Delete = 8
    }

    [Flags]
    public enum OrderB
    {
        Add = 1,
        Read = 2,
        Edit = 4,
        Delete = 8
    }

    [Flags]
    public enum DepartmentC
    {
        Add = 1,
        Edit = 2,
        Read = 4,
        Delete = 8
    }
}
