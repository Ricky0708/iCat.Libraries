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
        /// <typeparam name="TPrivilegeEnum"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebAuthorizationPermission<TPrivilegeEnum>(this IServiceCollection services) where TPrivilegeEnum : Enum
        {
            services.AddSingleton<IPrivilegeProvider<TPrivilegeEnum>, PrivilegeProvider<TPrivilegeEnum>>();
            services.AddScoped<IAuthorizationHandler, AuthorizationPermissionsHandler<TPrivilegeEnum>>();
            services.AddAuthorizationPermission<TPrivilegeEnum>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

        /// <summary>
        /// Register AuthorizationPermissionsHandler, DefaultPermissionProvider, DefaultPrivilegeProvider
        /// </summary>
        /// <typeparam name="TPrivilegeEnum"></typeparam>
        /// <typeparam name="TPrivilegeProviderType"></typeparam>
        /// <typeparam name="TPrivilegeProcessorType"></typeparam>
        /// <typeparam name="TClaimProcessorType"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebAuthorizationPermission<
              TPrivilegeEnum
            , TPrivilegeProviderType
            , TPrivilegeProcessorType
            , TClaimProcessorType>(
            this IServiceCollection services
            )
            where TPrivilegeEnum : Enum
            where TPrivilegeProviderType : IPrivilegeProvider<TPrivilegeEnum>
            where TPrivilegeProcessorType : IPrivilegeProcessor<TPrivilegeEnum>
            where TClaimProcessorType : IClaimProcessor<TPrivilegeEnum>

        {
            services.AddSingleton<IPrivilegeProvider<TPrivilegeEnum>>(p => p.GetRequiredService<TPrivilegeProviderType>());
            services.AddScoped<IAuthorizationHandler, AuthorizationPermissionsHandler<TPrivilegeEnum>>();
            services.AddAuthorizationPermission<TPrivilegeEnum, TPrivilegeProcessorType, TClaimProcessorType>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
    }
}
