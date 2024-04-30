using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Interfaces
{
    /// <summary>
    /// Localizer Factory
    /// </summary>
    public interface IStringLocalizerFactory : Microsoft.Extensions.Localization.IStringLocalizerFactory
    {
        /// <summary>
        /// Get StringLocalizer
        /// </summary>
        /// <returns></returns>
        IStringLocalizer Create();
    }
}
