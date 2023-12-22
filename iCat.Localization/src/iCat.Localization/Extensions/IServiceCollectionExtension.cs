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
        public static IServiceCollection AddRncLocalizationeService(this IServiceCollection services, IEnumerable<LocalizationMapping> localizationMappings, Options? options = null)
        {
            services.AddSingleton<IStringLocalizerFactory>(s => s.GetRequiredService<IiCatLocalizerFactory>());
            services.AddSingleton<IStringLocalizer>(s => s.GetRequiredService<IiCatStringLocalizer>());
            services.AddSingleton<IiCatLocalizerFactory, iCatLocalizerFactory>();
            services.AddSingleton<IiCatStringLocalizer>(s => new iCatStringLocalizer(s.GetRequiredService<IiCatLocalizationDataProvider>(), options));
            services.AddSingleton<IiCatLocalizationDataProvider>(s => new DefaultiCatLocalizationDataProvider(localizationMappings));

            return services;
        }

        /// <summary>
        /// Register language services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddiCatLocalizationeService(this IServiceCollection services, IiCatLocalizationDataProvider iCatLocalizationDataProvider, Options? options = null)
        {
            services.AddSingleton<IStringLocalizerFactory>(s => s.GetRequiredService<IiCatLocalizerFactory>());
            services.AddSingleton<IStringLocalizer>(s => s.GetRequiredService<IiCatStringLocalizer>());
            services.AddSingleton<IiCatLocalizerFactory, iCatLocalizerFactory>();
            services.AddSingleton<IiCatStringLocalizer>(s => new iCatStringLocalizer(s.GetRequiredService<IiCatLocalizationDataProvider>(), options));
            services.AddSingleton<IiCatLocalizationDataProvider>(s => iCatLocalizationDataProvider);
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
            services.RemoveAll(typeof(IiCatLocalizerFactory));
            services.RemoveAll(typeof(IiCatStringLocalizer));
            services.RemoveAll(typeof(IiCatLocalizationDataProvider));
            return services;
        }

    }
}
