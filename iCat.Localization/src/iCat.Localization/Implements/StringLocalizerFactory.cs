using iCat.Localization.Interfaces;
using iCat.Localization.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Localization.Implements
{
    public class StringLocalizerFactory : Interfaces.IStringLocalizerFactory
    {
        private readonly Interfaces.IStringLocalizer _processor;

        public StringLocalizerFactory(Interfaces.IStringLocalizer processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public Interfaces.IStringLocalizer Create()
        {
            return _processor;
        }


        public Microsoft.Extensions.Localization.IStringLocalizer Create(Type resourceSource)
        {
            return (Microsoft.Extensions.Localization.IStringLocalizer)Create();
        }

        public Microsoft.Extensions.Localization.IStringLocalizer Create(string baseName, string location)
        {
            return (Microsoft.Extensions.Localization.IStringLocalizer)Create();
        }
    }
}
