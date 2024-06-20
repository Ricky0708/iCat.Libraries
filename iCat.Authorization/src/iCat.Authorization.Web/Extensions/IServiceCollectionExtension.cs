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
        /// Register AuthorizationPermissionsHandler, DefaultPermissionProvider, DefaultPermitProvider
        /// </summary>
        /// <param name="services"></param>
        /// <param name="permitEnum"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebAuthorizationPermission(this IServiceCollection services, Type permitEnum)
        {
            services.AddSingleton<IPermitProvider, PermitProvider>();
            services.AddScoped<IAuthorizationHandler, AuthorizationPermissionsHandler>();
            services.AddAuthorizationPermission(permitEnum);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
