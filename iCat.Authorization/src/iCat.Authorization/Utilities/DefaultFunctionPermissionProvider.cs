﻿using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <inheritdoc/>
    public class DefaultFunctionPermissionProvider : IFunctionPermissionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FunctionPermissionParser _parser;
        private readonly HttpContext _httpContext;

        /// <inheritdoc/>
        public DefaultFunctionPermissionProvider(IHttpContextAccessor httpContextAccessor, FunctionPermissionParser parser)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _httpContext = _httpContextAccessor.HttpContext ?? throw new ArgumentException("Can't find HttpContext.");
        }

        /// <inheritdoc/>
        public IEnumerable<FunctionPermissionData> GetUserPermission()
        {
            var userPermission = _httpContextAccessor.HttpContext!.User.Claims.Where(p => p.Type == AuthorizationPermissionClaimTypes.Permission).Select(p =>
            {
                var functionPermission = p.Value.Split(",");
                if (!int.TryParse(functionPermission[0], out var functionValue)) throw new ArgumentException("Invalid Permission claims");
                if (!int.TryParse(functionPermission[1], out var permissionValue)) throw new ArgumentException("Invalid Permission claims");
                var function = _parser.GetFunctionPermissionDefinitions().FirstOrDefault(p => p.FunctionValue == functionValue) ?? throw new ArgumentException("Function in claims is not in function list");
                return new FunctionPermissionData
                {
                    FunctionValue = function.FunctionValue,
                    FunctionName = function.FunctionName,
                    PermissionDetails = function.PermissionDetails.Where(p => (p.Permission & permissionValue) > 0).ToList()
                };
            });
            return userPermission;
        }

        /// <inheritdoc/>
        public bool Validate(IEnumerable<FunctionPermissionData> ownPermissions, FunctionPermissionData permissionRequired)
        {
            if (ownPermissions.Any(p => p.FunctionValue == permissionRequired.FunctionValue && (p.Permissions & permissionRequired.Permissions) > 0))
            {
                return true;
            }
            return false;
        }
    }
}