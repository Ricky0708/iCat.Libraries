using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Extensions
{
    public static class AuthorizationPolicyBuilderExtension
    {
        /// <summary>
        /// Add AuthorizationPermissionsRequirement
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AuthorizationPolicyBuilder AddAuthorizationPermissionRequirment(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new AuthorizationPermissionsRequirement());
            return builder;
        }
    }
}
