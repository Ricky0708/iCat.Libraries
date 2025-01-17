﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Authorization.Providers.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.Json;
using iCat.Authorization.Web;
using iCat.Authorization.Models;

namespace iCat.Authorization.Providers.Implements.Tests
{
    [TestClass()]
    public class PrivilegeProcessorTests
    {
        [TestMethod()]
        public void GetPrivilegeDefinitionFromPermissionTest()
        {
            // arrange
            var parser = new PrivilegeProcessor<Privilege_Success>();
            var validationData =
                new PrivilegeTest()
                {
                    Name = nameof(UserProfileA),
                    Value = Privilege_Success.UserProfile,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = nameof( UserProfileA.Add), Value = (int) UserProfileA.Add },
                        new() { Name = nameof(UserProfileA.Edit), Value = (int)UserProfileA.Edit },
                        new() { Name = nameof(UserProfileA.Read), Value = (int)UserProfileA.Read },
                        new() { Name = nameof(UserProfileA.Delete), Value = (int)UserProfileA.Delete },
                }
                };

            // action
            var defintions = parser.GetPrivilegeDefinitionFromPermission(UserProfileA.Read | UserProfileA.Add);

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetPrivilegeDefinitionFromPermissionTest1()
        {
            // arrange
            var parser = new PrivilegeProcessor<Privilege_Success>();
            var validationData =
                new PrivilegeTest()
                {
                    Name = nameof(UserProfileA),
                    Value = Privilege_Success.UserProfile,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = nameof( UserProfileA.Add), Value = (int) UserProfileA.Add },
                        new() { Name = nameof(UserProfileA.Edit), Value = (int)UserProfileA.Edit },
                        new() { Name = nameof(UserProfileA.Read), Value = (int)UserProfileA.Read },
                        new() { Name = nameof(UserProfileA.Delete), Value = (int)UserProfileA.Delete },
                }
                };

            // action
            var defintions = parser.GetPrivilegeDefinitionFromPermission(typeof(UserProfileA));

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetPrivilegePermissionDefinitions_Success()
        {
            // arrange
            var parser = new PrivilegeProcessor<Privilege_Success>();
            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = Privilege_Success.UserProfile,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
                    Value = Privilege_Success.Order,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 2 },
                        new() { Name = "Edit", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
                    Value = Privilege_Success.Department,
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
            var parser = new PrivilegeProcessor<Privilege_Fail>();
            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = Privilege_Success.UserProfile,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
                    Value = Privilege_Success.Department,
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
            var provider = new PrivilegeProcessor<Privilege_Fail>();
            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = Privilege_Success.UserProfile,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(DepartmentC),
                    Value = Privilege_Success.Department,
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
        public void GetPermissionsAuthorizationDataTest_Success()
        {
            // arrange
            var parser = new PrivilegeProcessor<Privilege_Success>();
            var method = typeof(PrivilegeProcessorTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = Privilege_Success.UserProfile,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
                    Value = Privilege_Success.Order,
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
        public void GetPermissionsAuthorizationDataTest_Fail()
        {
            // arrange
            var parser = new PrivilegeProcessor<Privilege_Success>();
            var method = typeof(PrivilegeProcessorTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<PrivilegeTest> {
                new() {
                    Name = nameof(UserProfileA),
                    Value = Privilege_Success.UserProfile,
                    PermissionsData = new List<PermissionTest> {
                        new() { Name = "Add", Value = 1 },
                        new() { Name = "Read", Value = 4 },
                        new() { Name = "Edit", Value = 2 },
                        new() { Name = "Delete", Value = 8 },
                }},
                new() {
                    Name = nameof(OrderB),
                    Value = Privilege_Success.Order,
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
            var permissionProvider = new PrivilegeProcessor<Privilege_Success>();
            var claimGenerator = new ClaimProcessor<Privilege_Success>(permissionProvider);

            // action
            var result = claimGenerator.GeneratePrivilegeClaim(new PrivilegeTest
            {
                Name = "A",
                Value = Privilege_Success.UserProfile,
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
            var permissionProvider = new PrivilegeProcessor<Privilege_Success>();
            var claimGenerator = new ClaimProcessor<Privilege_Success>(permissionProvider);

            // action
            var result = claimGenerator.GeneratePrivilegeClaim(OrderB.Read);

            // assert
            Assert.AreEqual(result.Type, Constants.ClaimTypes.Privilege);
            Assert.AreEqual(result.Value, "2,2");
        }

        [PermissionsAuthorization(
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
    public class PrivilegeTest : Privilege<Privilege_Success>
    {
        /// <summary>
        /// Privilege name
        /// </summary>
        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Privilege value
        /// </summary>
        public new Privilege_Success Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// permission detail
        /// </summary>
        public new List<PermissionTest> PermissionsData
        {
            get
            {
                return base.PermissionsData.Select(p => new PermissionTest
                {
                    Name = p.Name,
                    Value = p.Value
                }).ToList();
            }
            set
            {
                base.PermissionsData = value.Select(p => (Permission)p).ToList();
            }
        }
    }

    /// <summary>
    /// Permission detail
    /// </summary>
    public class PermissionTest : Permission
    {
        /// <summary>
        /// Permission name
        /// </summary>
        public new string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Permission
        /// </summary>
        public new int Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }
    }

    public enum Privilege_Success
    {
        [PrivilegeDetail(typeof(UserProfileA))]
        UserProfile = 1,
        [PrivilegeDetail(typeof(OrderB))]
        Order = 2,
        [PrivilegeDetail(typeof(DepartmentC))]
        Department = 3
    }

    public enum Privilege_Fail
    {
        [PrivilegeDetail(typeof(UserProfileA))]
        UserProfile = 1,
        [PrivilegeDetail(typeof(DepartmentC))]
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