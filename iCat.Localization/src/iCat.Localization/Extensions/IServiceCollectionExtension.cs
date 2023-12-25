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
            services.AddSingleton((Func<IServiceProvider, IStringLocalizerFactory>)(s => s.GetRequiredService<Interfaces.LocalizerFactory>()));
            services.AddSingleton((Func<IServiceProvider, IStringLocalizer>)(s => s.GetRequiredService<Interfaces.StringLocalizer>()));
            services.AddSingleton<LocalizerFactory, LocalizerFactory>();
            services.AddSingleton((Func<IServiceProvider, Interfaces.StringLocalizer>)(s => new Implements.StringLocalizer(s.GetRequiredService<LocalizationDataProvider>(), options)));
            services.AddSingleton<LocalizationDataProvider>(s => new DefaultiCatLocalizationDataProvider(localizationMappings));

            return services;
        }

        /// <summary>
        /// Register language services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddiCatLocalizationeService(this IServiceCollection services, LocalizationDataProvider iCatLocalizationDataProvider, Options? options = null)
        {
            services.AddSingleton((Func<IServiceProvider, IStringLocalizerFactory>)(s => s.GetRequiredService<Interfaces.LocalizerFactory>()));
            services.AddSingleton((Func<IServiceProvider, IStringLocalizer>)(s => s.GetRequiredService<Interfaces.StringLocalizer>()));
            services.AddSingleton<LocalizerFactory, LocalizerFactory>();
            services.AddSingleton((Func<IServiceProvider, Interfaces.StringLocalizer>)(s => new Implements.StringLocalizer(s.GetRequiredService<LocalizationDataProvider>(), options)));
            services.AddSingleton<LocalizationDataProvider>(s => iCatLocalizationDataProvider);
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
            services.RemoveAll(typeof(IStringLocalizerFactory));
            services.RemoveAll(typeof(IStringLocalizer));
            services.RemoveAll(typeof(Interfaces.LocalizerFactory));
            services.RemoveAll(typeof(Interfaces.StringLocalizer));
            services.RemoveAll(typeof(LocalizationDataProvider));
            return services;
        }

    }
}
