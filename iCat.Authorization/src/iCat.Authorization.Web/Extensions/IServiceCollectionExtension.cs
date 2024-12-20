using iCat.Authorization.Providers.Interfaces;
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
using iCat.Authorization.Extensions;
using iCat.Authorization.Web.Providers.Implements;
using iCat.Authorization.Web.Providers.Interfaces;
namespace iCat.Authorization.Web.Extensions
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
        public static IServiceCollection AddWebAuthorizationPermission(this IServiceCollection services, Type privilegeEnum)
        {
            services.AddSingleton<IPrivilegeProvider, PrivilegeProvider>();
            services.AddScoped<IAuthorizationHandler, AuthorizationPermissionsHandler>();
            services.AddAuthorizationPermission(privilegeEnum);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
