using iCat.Authorization.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Extensions
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Register AuthorizationPermissionsHandler, DefaultUserPermissionProvider, FunctionPermissionParser
        /// </summary>
        /// <param name="services"></param>
        /// <param name="functionEnum"></param>
        /// <param name="functionPermissionEnums"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationPermission(this IServiceCollection services, Type functionEnum, params Type[] functionPermissionEnums)
        {
            services.AddSingleton<IAuthorizationHandler, AuthorizationPermissionsHandler>();
            services.AddSingleton<IUserPermissionProvider, DefaultUserPermissionProvider>();
            services.AddSingleton<FunctionPermissionParser>(p => new FunctionPermissionParser(functionEnum, functionPermissionEnums));
            return services;
        }
    }
}
