using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using iCat.Authorization.Web.Providers;

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
                new (Constants.ClaimTypes.Permit, $"{(int)Permit.UserProfile},{(int)(UserProfileQQ.Add | UserProfileQQ.Read)}"),
                new (Constants.ClaimTypes.Permit, $"{(int)Permit.Order},{(int)(OrderPe.Add | OrderPe.Read)}"),
            };
            var claimIdentity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(claimIdentity);
            var hc = Substitute.For<HttpContext>();
            hc.User = principal;
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext = hc;
            var permissionProvider = new PermissionProvider(typeof(Permit));
            var permitProvider = new PermitClaimProcessor(accessor, permissionProvider);

            var expeced = new List<PermitTest> {
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
            var result = permitProvider.GetPermits();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(expeced), JsonSerializer.Serialize(result));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserPermission_Duplicate_Test()
        {
            // arrange

            // action
            var permissionProvider = new PermissionProvider(typeof(Permit_Duplicate));

            // assert
        }

        [DataRow(Permit.UserProfile, UserProfileQQ.Add, true)]
        [DataRow(Permit.UserProfile, UserProfileQQ.Edit, true)]
        [DataRow(Permit.UserProfile, UserProfileQQ.Read, false)]
        [DataRow(Permit.UserProfile, UserProfileQQ.Delete, false)]
        [DataRow(Permit.UserProfile, UserProfileQQ.Delete | UserProfileQQ.Read, false)]
        [DataRow(Permit.UserProfile, UserProfileQQ.Delete | UserProfileQQ.Read | UserProfileQQ.Add, true)]
        [DataRow(Permit.Order, OrderPe.Add, false)]
        [TestMethod()]
        public void ValidateTest_Success(Permit permit, UserProfileQQ permission, bool expected)
        {
            // arrange
            var accessor = Substitute.For<IHttpContextAccessor>();
            var permissionProvider = new PermissionProvider(typeof(Permit));
            var permitProvider = new PermitClaimProcessor(accessor, permissionProvider);
            var userPermission = new List<PermitTest> {
                new() {
                    Value = (int)permit,
                    PermissionsData = new List<PermissionTest>
                    {
                        new(){
                            Value = (int)permission
                        }
                    }
                }
            };

            // action
            var result = permissionProvider.Validate(userPermission, new PermitTest
            {
                Value = (int)Permit.UserProfile,
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

    public enum Permit_Duplicate
    {
        [PermissionRelation(typeof(UserProfileQQ))]
        UserProfile = 1,
        [PermissionRelation(typeof(UserProfileQQ))]
        Order = 2,
        [PermissionRelation(typeof(DepartmentPP))]
        Department = 3
    }

    public enum Permit
    {
        [PermissionRelation(typeof(UserProfileQQ))]
        UserProfile = 1,
        [PermissionRelation(typeof(OrderPe))]
        Order = 2,
        [PermissionRelation(typeof(DepartmentPP))]
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