using iCat.Authorization.Models;
using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Providers.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Web.Providers
{
    /// <inheritdoc/>
    public class PermitClaimProcessor : BasePermitClaimProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <inheritdoc/>
        public PermitClaimProcessor(IHttpContextAccessor _httpContextAccessor, IPermissionProvider permissionProvider) : base(permissionProvider)
        {
            this._httpContextAccessor = _httpContextAccessor ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
        }

        /// <inheritdoc/>
        public override IEnumerable<Permit> GetPermits()
        {
            var userPermits = _httpContextAccessor?.HttpContext?.User.Claims.Where(p => p.Type == Constants.ClaimTypes.Permit).Select(p =>
            {
                var permission = p.Value.Split(",");
                if (!int.TryParse(permission[0], out var permitValue)) throw new ArgumentException("Invalid Permit claims");
                if (!int.TryParse(permission[1], out var permissionsValue)) throw new ArgumentException("Invalid Permit claims");
                return ExtractPermit(permitValue, permissionsValue);
            }) ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            return userPermits;
        }
    }
}
