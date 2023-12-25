using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Interfaces
{
    public interface IStringLocalizerFactory : Microsoft.Extensions.Localization.IStringLocalizerFactory
    {
        IStringLocalizer Create();
    }
}
