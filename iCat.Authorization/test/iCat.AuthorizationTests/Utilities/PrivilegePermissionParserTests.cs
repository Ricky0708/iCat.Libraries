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
using iCat.Authorization.Web;
using iCat.Authorization.Providers.Implements;

namespace iCat.Authorization.Utilities.Tests
{
    [TestClass()]
    public class PrivilegePermissionParserTests
    {
        [TestMethod()]
        public void GetPrivilegePermissionDefinitions_Success()
        {
            // arrange
            var parser = new PermissionProcessor(typeof(Privilege_Success));
            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
                    Value = 2,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 2 },
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
                    Value = 3,
                    PermissionsData = new List<PermissionTest> {
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
        public void GetPrivilegePermissionDefinitions_Fail1()
        {
            // arrange
            var parser = new PermissionProcessor(typeof(Privilege_Fail));
            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
                    Value = 3,
                    PermissionsData = new List<PermissionTest> {
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
        public void GetPrivilegePermissionDefinitions_Fail2()
        {
            // arrange
            var provider = new PermissionProcessor(typeof(Privilege_Fail));
            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
                    Value = 3,
                    PermissionsData = new List<PermissionTest> {
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
            var parser = new PermissionProcessor(typeof(Privilege_Success));
            var method = typeof(PrivilegePermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
                    Value = 2,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = parser.GetPrivilegeFromAttribute(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetAuthorizationPermissionsDataTest_Fail()
        {
            // arrange
            var parser = new PermissionProcessor(typeof(Privilege_Success));
            var method = typeof(PrivilegePermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = 1,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
                    Value = 2,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
            };

            // action
            var defintions = parser.GetPrivilegeFromAttribute(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetClaimFromPrivilegeDataTest()
        {
            // arrange
            var permissionProvider = new PermissionProcessor(typeof(Privilege_Success));
            var claimGenerator = new ClaimProcessor(permissionProvider);

            // action
            var result = claimGenerator.GeneratePrivilegeClaim(new PrivilegeTest
            {
                Name = "A",
                Value = 1,
                PermissionsData = new List<PermissionTest>
                {
                    new() { Name = "Add", Value = 1}
                }
            });

            // assert
            Assert.AreEqual(result.Type, Constants.ClaimTypes.Privilege);
            Assert.AreEqual(result.Value, "1,1");
        }

        [TestMethod()]
        public void GetClaimFromPrivilegeDataTest2()
        {
            // arrange
            var permissionProvider = new PermissionProcessor(typeof(Privilege_Success));
            var claimGenerator = new ClaimProcessor(permissionProvider);

            // action
            var result = claimGenerator.GeneratePrivilegeClaim(OrderB.Read);

            // assert
            Assert.AreEqual(result.Type, Constants.ClaimTypes.Privilege);
            Assert.AreEqual(result.Value, "2,2");
        }

        [AuthorizationPermissions(
        UserProfileA.Add | UserProfileA.Read,
        OrderB.Edit | OrderB.Delete,
        UserProfileA.Edit | UserProfileA.Delete)]
        public static void TestAttributeMethod()
        {

        }



    }

    #region test data

    /// <summary>
    /// Privilege - Permission information
    /// </summary>
    public class PrivilegeTest : IPrivilege<PermissionTest>
    {
        /// <summary>
        /// Privilege name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Privilege value
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// permission detail
        /// </summary>
        public List<PermissionTest> PermissionsData { get; set; } = new List<PermissionTest>();

        /// <summary>
        /// Permissions
        /// </summary>
        public int Permissions => PermissionsData?.Sum(p => p.Value) ?? 0;
    }

    /// <summary>
    /// Permission detail
    /// </summary>
    public class PermissionTest : IPermission
    {
        /// <summary>
        /// Permission name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Permission
        /// </summary>
        public int Value { get; set; }
    }

    public enum Privilege_Success
    {
        [PrivilegDetail(typeof(UserProfileA))]
        UserProfile = 1,
        [PrivilegDetail(typeof(OrderB))]
        Order = 2,
        [PrivilegDetail(typeof(DepartmentC))]
        Department = 3
    }

    public enum Privilege_Fail
    {
        [PrivilegDetail(typeof(UserProfileA))]
        UserProfile = 1,
        [PrivilegDetail(typeof(DepartmentC))]
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

    #endregion

}
