﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Authorization.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using System.Text.Json;
using iCat.Authorization.Utilities;
using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Web.Providers.Implements;

namespace iCat.Authorization.Providers.Tests
{
    [TestClass()]
    public class DefaultUserPermissionProviderTests
    {
        [TestMethod()]
        public void GetUserPermissionTest()
        {
            // arrange
            var claims = new List<Claim>() {
                new (Constants.ClaimTypes.Privilege, $"{(int)PrivilegeEnum.UserProfile},{(int)(UserProfileQQ.Add | UserProfileQQ.Read)}"),
                new (Constants.ClaimTypes.Privilege, $"{(int)PrivilegeEnum.Order},{(int)(OrderPe.Add | OrderPe.Read)}"),
            };
            var claimIdentity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(claimIdentity);
            var hc = Substitute.For<HttpContext>();
            hc.User = principal;
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext = hc;
            var permissionProcessor = new PermissionProcessor(typeof(PrivilegeEnum));
            var claimProcessor = new ClaimProcessor(permissionProcessor);
            var privilegeProvider = new PrivilegeProvider(accessor, claimProcessor, permissionProcessor);

            var expeced = new List<PrivilegeTest> {
                new() { Name = nameof(UserProfileQQ),
                    Value = 1,
                    PermissionsData = new List<PermissionTest> {
                        new() {
                            Name = "Add",
                            Value = 1,
                        },
                        new() {
                            Name = "Read",
                            Value = 4,
                        }
                    },
                },
                new() { Name = nameof(OrderPe),
                    Value = 2,
                    PermissionsData = new List<PermissionTest> {
                        new(){
                            Name = "Add",
                            Value = 1,
                        },
                        new() {
                            Name = "Read",
                            Value = 2,
                        }
                    },
                },
            };


            // action
            var result = privilegeProvider.GetCurrentUserPrivileges();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(expeced), JsonSerializer.Serialize(result));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserPermission_Duplicate_Test()
        {
            // arrange

            // action
            var permissionProvider = new PermissionProcessor(typeof(Privilege_Duplicate));

            // assert
        }

        [DataRow(PrivilegeEnum.UserProfile, UserProfileQQ.Add, true)]
        [DataRow(PrivilegeEnum.UserProfile, UserProfileQQ.Edit, true)]
        [DataRow(PrivilegeEnum.UserProfile, UserProfileQQ.Read, false)]
        [DataRow(PrivilegeEnum.UserProfile, UserProfileQQ.Delete, false)]
        [DataRow(PrivilegeEnum.UserProfile, UserProfileQQ.Delete | UserProfileQQ.Read, false)]
        [DataRow(PrivilegeEnum.UserProfile, UserProfileQQ.Delete | UserProfileQQ.Read | UserProfileQQ.Add, true)]
        [DataRow(PrivilegeEnum.Order, OrderPe.Add, false)]
        [TestMethod()]
        public void ValidateTest_Success(PrivilegeEnum privilege, UserProfileQQ permission, bool expected)
        {
            // arrange
            var accessor = Substitute.For<IHttpContextAccessor>();
            var permissionProvider = new PermissionProcessor(typeof(PrivilegeEnum));
            var PrivilegeProvider = new ClaimProcessor(permissionProvider);
            var userPermission = new List<PrivilegeTest> {
                new() {
                    Value = (int)privilege,
                    PermissionsData = new List<PermissionTest>
                    {
                        new(){
                            Value = (int)permission
                        }
                    }
                }
            };

            // action
            var result = permissionProvider.ValidatePermission(userPermission, new PrivilegeTest
            {
                Value = (int)PrivilegeEnum.UserProfile,
                PermissionsData = new List<PermissionTest> {
                    new(){
                        Value = (int)UserProfileQQ.Add,
                    },
                    new(){
                        Value = (int)UserProfileQQ.Edit,
                    }
                }
            });

            // assert

            Assert.AreEqual(expected, result);
        }
    }


    #region test data

    /// <summary>
    /// Privilege - Permission information
    /// </summary>
    public class PrivilegeTest : Privilege
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

    public enum Privilege_Duplicate
    {
        [PrivilegDetail(typeof(UserProfileQQ))]
        UserProfile = 1,
        [PrivilegDetail(typeof(UserProfileQQ))]
        Order = 2,
        [PrivilegDetail(typeof(DepartmentPP))]
        Department = 3
    }

    public enum PrivilegeEnum
    {
        [PrivilegDetail(typeof(UserProfileQQ))]
        UserProfile = 1,
        [PrivilegDetail(typeof(OrderPe))]
        Order = 2,
        [PrivilegDetail(typeof(DepartmentPP))]
        Department = 3
    }

    [Flags]
    public enum UserProfileQQ
    {
        Add = 1,
        Edit = 2,
        Read = 4,
        Delete = 8
    }

    [Flags]
    public enum OrderPe
    {
        Add = 1,
        Read = 2,
        Edit = 4,
        Delete = 8
    }

    [Flags]
    public enum DepartmentPP
    {
        Add = 1,
        Edit = 2,
        Read = 4,
        Delete = 8
    }

    #endregion

}