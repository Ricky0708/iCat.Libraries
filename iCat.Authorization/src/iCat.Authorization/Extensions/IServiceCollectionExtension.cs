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
        /// <typeparam name="TPrivilegeEnum"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationPermission<TPrivilegeEnum>(this IServiceCollection services) where TPrivilegeEnum : Enum
        {
            services.AddSingleton<IPrivilegeProcessor<TPrivilegeEnum>, PrivilegeProcessor<TPrivilegeEnum>>();
            services.AddSingleton<IClaimProcessor<TPrivilegeEnum>, ClaimProcessor<TPrivilegeEnum>>();
            return services;
        }

        /// <summary>
        /// Register AuthorizationPermissionsHandler, DefaultPermissionProvider, DefaultPrivilegeProvider
        /// </summary>
        /// <typeparam name="TPrivilegeEnum"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationPermission<
              TPrivilegeEnum
            , TPrivilegeProcessorType
            , TClaimProcessorType>(
            this IServiceCollection services
            )
            where TPrivilegeEnum : Enum
            where TPrivilegeProcessorType : IPrivilegeProcessor<TPrivilegeEnum>
            where TClaimProcessorType : IClaimProcessor<TPrivilegeEnum>
        {
            services.AddSingleton<IPrivilegeProcessor<TPrivilegeEnum>>(p => p.GetRequiredService<TPrivilegeProcessorType>());
            services.AddSingleton<IClaimProcessor<TPrivilegeEnum>>(p => p.GetRequiredService<TClaimProcessorType>()); ;
            return services;
        }
    }
}
