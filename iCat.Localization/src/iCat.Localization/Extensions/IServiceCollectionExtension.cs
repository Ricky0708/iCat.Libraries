using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using iCat.Localization.Interfaces;
using iCat.Localization.Implements;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using iCat.Localization.Models;

namespace iCat.Localization.Extensions
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Register language services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddiCatLocalizationeService(this IServiceCollection services, IEnumerable<LocalizationMapping> localizationMappings, Options? options = null)
        {
            services.AddSingleton((Func<IServiceProvider, Microsoft.Extensions.Localization.IStringLocalizerFactory>)(s => s.GetRequiredService<Interfaces.IStringLocalizerFactory>()));
            services.AddSingleton((Func<IServiceProvider, Microsoft.Extensions.Localization.IStringLocalizer>)(s => s.GetRequiredService<Interfaces.IStringLocalizer>()));
            services.AddSingleton<Interfaces.IStringLocalizerFactory, StringLocalizerFactory>();
            services.AddSingleton((Func<IServiceProvider, Interfaces.IStringLocalizer>)(s => new Implements.StringLocalizer(s.GetRequiredService<IStringLocalizationDataProvider>(), options)));
            services.AddSingleton<IStringLocalizationDataProvider>(s => new DefaultiCatLocalizationDataProvider(localizationMappings));

            return services;
        }

        /// <summary>
        /// Register language services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddiCatLocalizationeService(this IServiceCollection services, IStringLocalizationDataProvider iCatLocalizationDataProvider, Options? options = null)
        {
            services.AddSingleton((Func<IServiceProvider, Microsoft.Extensions.Localization.IStringLocalizerFactory>)(s => s.GetRequiredService<Interfaces.IStringLocalizerFactory>()));
            services.AddSingleton((Func<IServiceProvider, Microsoft.Extensions.Localization.IStringLocalizer>)(s => s.GetRequiredService<Interfaces.IStringLocalizer>()));
            services.AddSingleton<Interfaces.IStringLocalizerFactory, StringLocalizerFactory>();
            services.AddSingleton((Func<IServiceProvider, Interfaces.IStringLocalizer>)(s => new Implements.StringLocalizer(s.GetRequiredService<IStringLocalizationDataProvider>(), options)));
            services.AddSingleton<IStringLocalizationDataProvider>(s => iCatLocalizationDataProvider);
            return services;
        }

        /// <summary>
        /// Remove registered language services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RemoveiCatLocalizationService(this IServiceCollection services)
        {
            //services.Where(p => p.ServiceType == typeof(IStringLocalizerFactory));
            services.RemoveAll(typeof(Microsoft.Extensions.Localization.IStringLocalizerFactory));
            services.RemoveAll(typeof(Microsoft.Extensions.Localization.IStringLocalizer));
            services.RemoveAll(typeof(Interfaces.IStringLocalizerFactory));
            services.RemoveAll(typeof(Interfaces.IStringLocalizer));
            services.RemoveAll(typeof(IStringLocalizationDataProvider));
            return services;
        }

    }
}
