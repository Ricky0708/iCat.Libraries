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
    public class PermitPermissionParserTests
    {
        [TestMethod()]
        public void GetPermitPermissionDefinitions_Success()
        {
            // arrange
            var parser = new PermissionProcessor(typeof(Permit_Success));
            var validationData = new List<PermitTest> {
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
        public void GetPermitPermissionDefinitions_Fail1()
        {
            // arrange
            var parser = new PermissionProcessor(typeof(Permit_Fail));
            var validationData = new List<PermitTest> {
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
        public void GetPermitPermissionDefinitions_Fail2()
        {
            // arrange
            var provider = new PermissionProcessor(typeof(Permit_Fail));
            var validationData = new List<PermitTest> {
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
            var parser = new PermissionProcessor(typeof(Permit_Success));
            var method = typeof(PermitPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<PermitTest> {
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
            var defintions = parser.GetPermitFromAttribute(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetAuthorizationPermissionsDataTest_Fail()
        {
            // arrange
            var parser = new PermissionProcessor(typeof(Permit_Success));
            var method = typeof(PermitPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<PermitTest> {
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
            var defintions = parser.GetPermitFromAttribute(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetClaimFromPermitDataTest()
        {
            // arrange
            var permissionProvider = new PermissionProcessor(typeof(Permit_Success));
            var claimGenerator = new ClaimProcessor(permissionProvider);

            // action
            var result = claimGenerator.GeneratePermitClaim(new PermitTest
            {
                Name = "A",
                Value = 1,
                PermissionsData = new List<PermissionTest>
                {
                    new() { Name = "Add", Value = 1}
                }
            });

            // assert
            Assert.AreEqual(result.Type, Constants.ClaimTypes.Permit);
            Assert.AreEqual(result.Value, "1,1");
        }

        [TestMethod()]
        public void GetClaimFromPermitDataTest2()
        {
            // arrange
            var permissionProvider = new PermissionProcessor(typeof(Permit_Success));
            var claimGenerator = new ClaimProcessor(permissionProvider);

            // action
            var result = claimGenerator.GeneratePermitClaim(OrderB.Read);

            // assert
            Assert.AreEqual(result.Type, Constants.ClaimTypes.Permit);
            Assert.AreEqual(result.Value, "2,2");
        }

        [TestMethod()]
        public void GetClaimFromPermitDataTest3()
        {
            // arrange
            var permissionProvider = new PermissionProcessor(typeof(Permit_Success));
            var claimGenerator = new ClaimProcessor(permissionProvider);

            // action
            var result = claimGenerator.GeneratePermitClaim(2, 2);

            // assert
            Assert.AreEqual(result.Type, Constants.ClaimTypes.Permit);
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
    /// Permit - Permission information
    /// </summary>
    public class PermitTest : IPermit<PermissionTest>
    {
        /// <summary>
        /// Permit name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Permit value
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

    public enum Permit_Success
    {
        [PermissionRelation(typeof(UserProfileA))]
        UserProfile = 1,
        [PermissionRelation(typeof(OrderB))]
        Order = 2,
        [PermissionRelation(typeof(DepartmentC))]
        Department = 3
    }

    public enum Permit_Fail
    {
        [PermissionRelation(typeof(UserProfileA))]
        UserProfile = 1,
        [PermissionRelation(typeof(DepartmentC))]
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
