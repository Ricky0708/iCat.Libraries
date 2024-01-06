using iCat.Authorization.Constants;
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
    public class DefaultUserPermissionProvider : IUserPermissionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FunctionPermissionParser _parser;
        private readonly HttpContext _httpContext;

        /// <inheritdoc/>
        public DefaultUserPermissionProvider(IHttpContextAccessor httpContextAccessor, FunctionPermissionParser parser)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _httpContext = _httpContextAccessor.HttpContext ?? throw new ArgumentException("Can't find HttpContext.");
        }

        /// <inheritdoc/>
        public IEnumerable<FunctionData> GetUserPermission()
        {
            var userPermission = _httpContext.User.Claims.Where(p => p.Type == AuthorizationPermissionClaimTypes.Permission).Select(p =>
            {
                var functionPermission = p.Value.Split(",");
                if (!int.TryParse(functionPermission[0], out var functionValue)) throw new ArgumentException("Invalid Permission claims");
                if (!int.TryParse(functionPermission[1], out var permissionValue)) throw new ArgumentException("Invalid Permission claims");
                var function = _parser.GetFunctionPermissionDefinitions().FirstOrDefault(p => p.FunctionValue == functionValue) ?? throw new ArgumentException("Function in claims is not in function list");
                return new FunctionData
                {
                    FunctionValue = function.FunctionValue,
                    FunctionName = function.FunctionName,
                    PermissionDetails = function.PermissionDetails.Where(p => (p.Permission & permissionValue) > 0).ToList()
                };
            });
            return userPermission;
        }

        /// <inheritdoc/>
        public bool Validate(IEnumerable<FunctionData> ownPermissions, FunctionData permissionRequired)
        {
            if (ownPermissions.Any(p => p.FunctionValue == permissionRequired.FunctionValue && (p.Permissions & permissionRequired.Permissions) > 0))
            {
                return true;
            }
            return false;
        }
    }
}
