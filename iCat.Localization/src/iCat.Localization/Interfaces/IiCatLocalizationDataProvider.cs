using iCat.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Interfaces
{
    public interface IiCatLocalizationDataProvider
    {
        delegate void UpdateHandler();
        event UpdateHandler? NotifyUpdate;
        IEnumerable<LocalizationMapping> GetLanguageMappingData();
    }
}
