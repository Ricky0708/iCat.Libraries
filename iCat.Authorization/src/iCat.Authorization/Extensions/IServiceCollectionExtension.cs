using iCat.Authorization.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        /// Register AuthorizationPermissionsHandler, DefaultPermissionProvider, DefaultPermitProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="permitEnum"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationPermission(this IServiceCollection services, Type permitEnum)
        {
            services.AddScoped<IAuthorizationHandler, AuthorizationPermissionsHandler>();
            services.AddSingleton<IPermissionProvider>(s => new DefaultPermissionProvider(permitEnum));
            services.AddSingleton<IPermitProvider, DefaultPermitProvider>();
            return services;
        }
    }
}
