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

namespace iCat.Authorization.Tests
{
    [TestClass()]
    public class FunctionPermissionParserTests
    {
        [TestMethod()]
        public void GetFunctionPermissionDefinitions_Success()
        {
            // arrange
            var parser = new FunctionPermissionParser(typeof(Function_Success), typeof(UserProfilePermission), typeof(OrderPermission), typeof(DepartmentPermission));
            var validationData = new List<FunctionPermissionData> {
                new FunctionPermissionData {
                    FunctionName = "UserProfile",
                    FunctionValue = 1,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
                new FunctionPermissionData {
                    FunctionName = "Order",
                    FunctionValue = 2,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Read", Permission = 2 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
                new FunctionPermissionData {
                    FunctionName = "Department",
                    FunctionValue = 3,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
            };

            // action
            var defintions = parser.GetFunctionPermissionDefinitions();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFunctionPermissionDefinitions_Fail1()
        {
            // arrange
            var parser = new FunctionPermissionParser(typeof(Function_Fail), typeof(UserProfilePermission), typeof(OrderPermission), typeof(DepartmentPermission));
            var validationData = new List<FunctionPermissionData> {
                new FunctionPermissionData {
                    FunctionName = "UserProfile",
                    FunctionValue = 1,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
                new FunctionPermissionData {
                    FunctionName = "Order",
                    FunctionValue = 2,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Read", Permission = 2 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
                new FunctionPermissionData {
                    FunctionName = "Department",
                    FunctionValue = 3,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
            };

            // action
            var defintions = parser.GetFunctionPermissionDefinitions();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetFunctionPermissionDefinitions_Fail2()
        {
            // arrange
            var parser = new FunctionPermissionParser(typeof(Function_Fail), typeof(UserProfilePermission), typeof(DepartmentPermission));
            var validationData = new List<FunctionPermissionData> {
                new FunctionPermissionData {
                    FunctionName = "UserProfile",
                    FunctionValue = 1,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
                new FunctionPermissionData {
                    FunctionName = "Department",
                    FunctionValue = 3,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
            };

            // action
            var defintions = parser.GetFunctionPermissionDefinitions();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        public void GetAuthorizationPermissionsDataTest_Success()
        {
            // arrange
            var parser = new FunctionPermissionParser(typeof(Function_Success), typeof(UserProfilePermission), typeof(OrderPermission), typeof(DepartmentPermission));
            var method = typeof(FunctionPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<FunctionPermissionData> {
                new FunctionPermissionData {
                    FunctionName = "UserProfile",
                    FunctionValue = 1,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
                new FunctionPermissionData {
                    FunctionName = "Order",
                    FunctionValue = 2,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Edit", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
            };

            // action
            var defintions = parser.GetAuthorizationPermissionsData(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetAuthorizationPermissionsDataTest_Fail()
        {
            // arrange
            var parser = new FunctionPermissionParser(typeof(Function_Success), typeof(UserProfilePermission), typeof(DepartmentPermission));
            var method = typeof(FunctionPermissionParserTests).GetMethod(nameof(TestAttributeMethod), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var validationData = new List<FunctionPermissionData> {
                new FunctionPermissionData {
                    FunctionName = "UserProfile",
                    FunctionValue = 1,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Add", Permission = 1 },
                        new PermissionDetail { PermissionName = "Read", Permission = 4 },
                        new PermissionDetail { PermissionName = "Edit", Permission = 2 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
                new FunctionPermissionData {
                    FunctionName = "Order",
                    FunctionValue = 2,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail { PermissionName = "Edit", Permission = 4 },
                        new PermissionDetail { PermissionName = "Delete", Permission = 8 },
                }},
            };

            // action
            var defintions = parser.GetAuthorizationPermissionsData(method!.CustomAttributes.ToArray());

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(validationData), JsonSerializer.Serialize(defintions));
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
        UserProfile = 1,
        Order = 2,
        Department = 3
    }

    public enum Function_Fail
    {
        UserProfile = 1,
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