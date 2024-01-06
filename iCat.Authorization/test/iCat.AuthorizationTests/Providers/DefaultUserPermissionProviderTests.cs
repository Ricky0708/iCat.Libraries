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
            var parser = new FunctionPermissionParser(typeof(Function), typeof(UserProfilePermission), typeof(OrderPermission), typeof(DepartmentPermission));
            var claims = new List<Claim>() {
                new Claim(AuthorizationPermissionClaimTypes.Permission, $"{(int)Function.UserProfile},{(int)(UserProfilePermission.Add | UserProfilePermission.Read)}"),
                new Claim(AuthorizationPermissionClaimTypes.Permission, $"{(int)Function.Order},{(int)(OrderPermission.Add | OrderPermission.Read)}"),
            };
            var claimIdentity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(claimIdentity);
            var hc = Substitute.For<HttpContext>();
            hc.User = principal;
            var accessor = Substitute.For<IHttpContextAccessor>();
            accessor.HttpContext = hc;

            var expeced = new List<FunctionData> {
                new FunctionData { FunctionName = "UserProfile",
                    FunctionValue = 1,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail{
                            PermissionName = "Add",
                            Permission = 1,
                        },
                        new PermissionDetail {
                            PermissionName = "Read",
                            Permission = 4,
                        }
                    },
                },
                new FunctionData { FunctionName = "Order",
                    FunctionValue = 2,
                    PermissionDetails = new List<PermissionDetail> {
                        new PermissionDetail{
                            PermissionName = "Add",
                            Permission = 1,
                        },
                        new PermissionDetail {
                            PermissionName = "Read",
                            Permission = 2,
                        }
                    },
                },
            };

            var provider = new DefaultUserPermissionProvider(accessor, parser);

            // action
            var result = provider.GetUserPermission();

            // assert
            Assert.AreEqual(JsonSerializer.Serialize(expeced), JsonSerializer.Serialize(result));
        }

        [DataRow(Function.UserProfile, UserProfilePermission.Add, true)]
        [DataRow(Function.UserProfile, UserProfilePermission.Edit, true)]
        [DataRow(Function.UserProfile, UserProfilePermission.Read, false)]
        [DataRow(Function.UserProfile, UserProfilePermission.Delete, false)]
        [DataRow(Function.UserProfile, UserProfilePermission.Delete | UserProfilePermission.Read, false)]
        [DataRow(Function.UserProfile, UserProfilePermission.Delete | UserProfilePermission.Read | UserProfilePermission.Add, true)]
        [DataRow(Function.Order, OrderPermission.Add, false)]
        [TestMethod()]
        public void ValidateTest_Success(Function userFunction, UserProfilePermission permission, bool expected)
        {
            // arrange
            var parser = new FunctionPermissionParser(typeof(Function), typeof(UserProfilePermission), typeof(OrderPermission), typeof(DepartmentPermission));
            var accessor = Substitute.For<IHttpContextAccessor>();
            var provider = new DefaultUserPermissionProvider(accessor, parser);
            var userPermission = new List<FunctionData> {
                new FunctionData {
                    FunctionValue = (int)userFunction,
                    PermissionDetails = new List<PermissionDetail>
                    {
                        new PermissionDetail{
                            Permission = (int)permission
                        }
                    }
                }
            };

            // action
            var result = provider.Validate(userPermission, new FunctionData
            {
                FunctionValue = (int)Function.UserProfile,
                PermissionDetails = new List<PermissionDetail> {
                    new PermissionDetail{
                        Permission = (int)UserProfilePermission.Add,
                    },
                    new PermissionDetail{
                        Permission = (int)UserProfilePermission.Edit,
                    }
                }
            });

            // assert

            Assert.AreEqual(expected, result);
        }
    }

    public enum Function
    {
        UserProfile = 1,
        Order = 2,
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