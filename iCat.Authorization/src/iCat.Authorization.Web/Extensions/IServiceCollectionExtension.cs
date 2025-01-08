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
using iCat.Authorization.Providers.Implements;
namespace iCat.Authorization.Web.Extensions
{
    /// <summary>
    /// IServiceCollection extension for PermissionAuthorization
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Register PermissionsAuthorizationHandler, DefaultPermissionProvider, DefaultPrivilegeProvider
        /// </summary>
        /// <typeparam name="TPrivilegeEnum"></typeparam>
        /// <param name="services"></param>
        /// <param name="withPermissionAuthorizationHandler"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebPermissionAuthorization<TPrivilegeEnum>(this IServiceCollection services, bool withPermissionAuthorizationHandler = true) where TPrivilegeEnum : Enum
        {
            AddWebPermissionAuthorizationProvider<TPrivilegeEnum, PrivilegeProvider<TPrivilegeEnum>, PrivilegeProcessor<TPrivilegeEnum>, ClaimProcessor<TPrivilegeEnum>>(services, withPermissionAuthorizationHandler);
            return services;
        }

        /// <summary>
        /// Register PermissionsAuthorizationHandler, DefaultPermissionProvider, DefaultPrivilegeProvider
        /// </summary>
        /// <typeparam name="TPrivilegeEnum"></typeparam>
        /// <typeparam name="TPrivilegeProviderType"></typeparam>
        /// <typeparam name="TPrivilegeProcessorType"></typeparam>
        /// <typeparam name="TClaimProcessorType"></typeparam>
        /// <param name="services"></param>
        /// <param name="withPermissionAuthorizationHandler"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebPermissionAuthorizationProvider<
              TPrivilegeEnum
            , TPrivilegeProviderType
            , TPrivilegeProcessorType
            , TClaimProcessorType>(
            this IServiceCollection services
            , bool withPermissionAuthorizationHandler = true
            )
            where TPrivilegeEnum : Enum
            where TPrivilegeProviderType : IPrivilegeProvider<TPrivilegeEnum>
            where TPrivilegeProcessorType : IPrivilegeProcessor<TPrivilegeEnum>
            where TClaimProcessorType : IClaimProcessor<TPrivilegeEnum>

        {
            services.AddSingleton<IPrivilegeProvider<TPrivilegeEnum>>(p => p.GetRequiredService<TPrivilegeProviderType>());
            services.AddWebPermissionAuthorizationProcessor<TPrivilegeEnum, TPrivilegeProcessorType, TClaimProcessorType>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            if (withPermissionAuthorizationHandler) services.AddScoped<IAuthorizationHandler, PermissionsAuthorizationHandler<TPrivilegeEnum>>();
            return services;
        }
    }
}
