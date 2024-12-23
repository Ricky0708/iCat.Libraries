﻿using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using iCat.Authorization.Providers.Interfaces;
using iCat.Authorization.Web.Providers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace iCat.Authorization.Web
{
    /// <summary>
    /// Authorize AuthorizationPermissionsRequirement
    /// </summary>
    public class AuthorizationPermissionsHandler : AuthorizationHandler<AuthorizationPermissionsRequirement>
    {
        private const string _endWith = "Permission";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPrivilegeProvider _privilegeProvider;

        /// <summary>
        /// Authorize AuthorizationPermissionsRequirement
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="privilegeProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AuthorizationPermissionsHandler(
            IHttpContextAccessor httpContextAccessor,
            IPrivilegeProvider privilegeProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _privilegeProvider = privilegeProvider ?? throw new ArgumentNullException(nameof(privilegeProvider));
        }

        /// <summary>
        /// handler
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationPermissionsRequirement requirement)
        {
            if (!context.User.Identity?.IsAuthenticated ?? false) { context.Fail(); return; }

            if (context.Resource is HttpContext httpContext)
            {
                var endpoint = httpContext.GetEndpoint()!;
                var routerPrivileges = _privilegeProvider.GetRouterPrivilegesRequired(endpoint);
                var userPrivilege = _privilegeProvider.GetCurrentUserPrivileges();
                foreach (var routerPrivilege in routerPrivileges)
                {
                    if (_privilegeProvider.ValidatePermission(userPrivilege, routerPrivilege))
                    {
                        context.Succeed(requirement);
                        await Task.FromResult(0);
                        return;
                    }
                }
                context.Fail();
            }
            else
            {
                context.Fail();
            }
            await Task.FromResult(0);
        }



    }
}
