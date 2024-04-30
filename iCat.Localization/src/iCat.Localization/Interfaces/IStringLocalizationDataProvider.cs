using iCat.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Interfaces
{
    /// <summary>
    /// Localization Data Provider
    /// </summary>
    public interface IStringLocalizationDataProvider
    {
        /// <summary>
        /// Notify StringLocalizer to update cache data
        /// </summary>
        delegate void UpdateHandler();

        /// <summary>
        /// Notify StringLocalizer update event
        /// </summary>
        event UpdateHandler? NotifyUpdate;

        /// <summary>
        /// Provide new mapping data
        /// </summary>
        /// <returns></returns>
        IEnumerable<LocalizationMapping> GetLanguageMappingData();
    }
}
