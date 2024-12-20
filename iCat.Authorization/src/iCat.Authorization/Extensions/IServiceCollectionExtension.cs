using iCat.Authorization.Providers.Implements;
using iCat.Authorization.Providers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Extensions
{
    /// <summary>
    /// IServiceCollection extension for AuthorizationPermission
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Register AuthorizationPermissionsHandler, DefaultPermissionProvider, DefaultPrivilegeProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="privilegeEnum"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationPermission(this IServiceCollection services, Type privilegeEnum)
        {
            services.AddSingleton<IPermissionProcessor>(s => new PermissionProcessor(privilegeEnum));
            services.AddSingleton<IClaimProcessor, ClaimProcessor>();
            return services;
        }
    }
}
