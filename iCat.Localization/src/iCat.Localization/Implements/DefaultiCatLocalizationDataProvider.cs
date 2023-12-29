using iCat.Localization.Interfaces;
using iCat.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Implements
{
    /// <summary>
    /// In order to reduce network latency, so data caching is implemented in Localizer, not here
    /// </summary>
    public class DefaultiCatLocalizationDataProvider : IStringLocalizationDataProvider
    {
        private readonly IEnumerable<LocalizationMapping> _localizationMappings;

        /// Call NotifyRefresh to notify the Localizer to update the cache
        public event IStringLocalizationDataProvider.UpdateHandler? NotifyUpdate;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="localizationMappings"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultiCatLocalizationDataProvider(IEnumerable<LocalizationMapping> localizationMappings)
        {
            _localizationMappings = localizationMappings ?? throw new ArgumentNullException(nameof(localizationMappings));
            NotifyUpdate?.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LocalizationMapping> GetLanguageMappingData()
        {
            var result = _localizationMappings;
            return result;
        }
    }
}
