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
        /// Register AuthorizationPermissionsHandler, DefaultUserPermissionProvider, FunctionPermissionParser
        /// </summary>
        /// <param name="services"></param>
        /// <param name="functionEnum"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationPermission(this IServiceCollection services, Type functionEnum)
        {
            services.AddSingleton<IAuthorizationHandler, AuthorizationPermissionsHandler>();
            services.AddSingleton<IFunctionPermissionProvider>(s => new DefaultFunctionPermissionProvider(s.GetRequiredService<IHttpContextAccessor>(), functionEnum));
            return services;
        }
    }
}
