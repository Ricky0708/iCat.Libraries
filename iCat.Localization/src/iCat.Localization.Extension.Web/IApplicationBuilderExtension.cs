using iCat.Localization.Extensions;
using iCat.Localization.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Extension.Web
{
    public static class IApplicationBuilderExtension
    {
        /// <summary>
        /// Set string extension for localization
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication UseiCatLocalizationExtension(this WebApplication app)
        {
            iCatLocalizationStringExtension.SetLocalizationFactory(app.Services.GetRequiredService<IiCatLocalizerFactory>());
            app.UseRequestLocalization();
            return app;
        }
    }
}
