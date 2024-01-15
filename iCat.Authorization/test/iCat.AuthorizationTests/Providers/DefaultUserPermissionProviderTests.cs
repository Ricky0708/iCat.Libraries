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
                new (AuthorizationPermissionClaimTypes.Permit, $"{(int)Permit.UserProfile},{(int)(UserProfileQQ.Add | UserProfileQQ.Read)}"),
                new (AuthorizationPermissionClaimTypes.Permit, $"{(int)Permit.Order},{(int)(OrderPe.Add | OrderPe.Read)}"),
            };
            var claimIdentity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(claimIdentity);
            var hc = Substitute.For<HttpContext>();
            hc.User = principal;
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext = hc;
            var permissionProvider = new DefaultPermissionProvider(typeof(Permit));
            var permitProvider = new DefaultPermitProvider(accessor, permissionProvider);

            var expeced = new List<Models.Permit> {
                new() { Name = nameof(UserProfileQQ),
                    Value = 1,
                    PermissionsData = new List<Permission> {
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
                    PermissionsData = new List<Permission> {
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
            var result = permitProvider.GetPermit();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(expeced), JsonSerializer.Serialize(result));
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
            var permissionProvider = new DefaultPermissionProvider(typeof(Permit));
            var permitProvider = new DefaultPermitProvider(accessor, permissionProvider);
            var userPermission = new List<Models.Permit> {
                new() {
                    Value = (int)permit,
                    PermissionsData = new List<Permission>
                    {
                        new(){
                            Value = (int)permission
                        }
                    }
                }
            };

            // action
            var result = permitProvider.Validate(userPermission, new Models.Permit
            {
                Value = (int)Permit.UserProfile,
                PermissionsData = new List<Permission> {
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

    public enum Permit
    {
        [Permission(typeof(UserProfileQQ))]
        UserProfile = 1,
        [Permission(typeof(OrderPe))]
        Order = 2,
        [Permission(typeof(DepartmentPP))]
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
}