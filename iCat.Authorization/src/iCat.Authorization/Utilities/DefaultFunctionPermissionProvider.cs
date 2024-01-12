using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <inheritdoc/>
    public class DefaultFunctionPermissionProvider : IFunctionPermissionProvider
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly FunctionPermissionParser _parser;

        /// <inheritdoc/>
        public DefaultFunctionPermissionProvider(IHttpContextAccessor? httpContextAccessor, Type functionEnum)
        {
            _httpContextAccessor = httpContextAccessor;
            _parser = new FunctionPermissionParser(functionEnum);
        }

        /// <inheritdoc/>
        public List<Function> GetAuthorizationPermissionsData(params CustomAttributeData[] attributes)
        {
            return _parser.GetAuthorizationPermissionsData(attributes);
        }

        /// <inheritdoc/>
        public Claim GetClaimFromFunctionPermissionData(Function functionPermissionData)
        {
            return _parser.GetClaimFromFunctionPermissionData(functionPermissionData);
        }

        /// <inheritdoc/>
        public List<Function>? GetFunctionPermissionDefinitions()
        {
            return _parser.GetFunctionPermissionDefinitions();
        }

        /// <inheritdoc/>
        public IEnumerable<Function> GetUserPermission()
        {
            var userPermission = _httpContextAccessor?.HttpContext?.User.Claims.Where(p => p.Type == AuthorizationPermissionClaimTypes.Permission).Select(p =>
            {
                var functionPermission = p.Value.Split(",");
                if (!int.TryParse(functionPermission[0], out var functionValue)) throw new ArgumentException("Invalid Permission claims");
                if (!int.TryParse(functionPermission[1], out var permissionValue)) throw new ArgumentException("Invalid Permission claims");
                var function = _parser.GetFunctionPermissionDefinitions().FirstOrDefault(p => p.Value == functionValue) ?? throw new ArgumentException("Function in claims is not in function list");
                return new Function
                {
                    Value = function.Value,
                    Name = function.Name,
                    PermissionsData = function.PermissionsData.Where(p => (p.Value & permissionValue) > 0).ToList()
                };
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPermission;
        }

        /// <inheritdoc/>
        public bool Validate(IEnumerable<Function> ownPermissions, Function permissionRequired)
        {
            if (ownPermissions.Any(p => p.Value == permissionRequired.Value && (p.Permissions & permissionRequired.Permissions) > 0))
            {
                return true;
            }
            return false;
        }
    }
}
