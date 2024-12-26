using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Web.Extensions
{
    /// <summary>
    /// AuthorizationPolicyBuilder extension for PermissionsAuthorizationRequirement
    /// </summary>
    public static class AuthorizationPolicyBuilderExtension
    {
        /// <summary>
        /// Add PermissionsAuthorizationRequirement
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AuthorizationPolicyBuilder AddPermissionsAuthorizationRequirment(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new PermissionsAuthorizationRequirement());
            return builder;
        }
    }
}
