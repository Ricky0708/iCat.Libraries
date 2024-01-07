using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.Globalization;
using iCat.Localization.Interfaces;
using iCat.Localization.Implements;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using iCat.Localization.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;

namespace iCat.Localization.Extension.Web
{
    /// <summary>
    /// 
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Request localization options
        /// 1.Route
        /// 2.QueryString
        /// 3.Cookie
        /// 4.AcceptLanguageHeader
        /// </summary>
        /// <param name="services"></param>
        /// <param name="supportedCultures"></param>
        /// <param name="keyName">key name for get culture from route / queryString / cookie</param>
        /// <returns></returns>
        public static IServiceCollection AddRequestLocalizationOptions(this IServiceCollection services, CultureInfo[] supportedCultures, string keyName)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures.First().Name, uiCulture: supportedCultures.First().Name);

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new RouteDataRequestCultureProvider { RouteDataStringKey = keyName, UIRouteDataStringKey = keyName },
                    new QueryStringRequestCultureProvider { QueryStringKey = keyName, UIQueryStringKey = keyName },
                    new CookieRequestCultureProvider { CookieName = keyName, },
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });
            return services;
        }
    }
}
