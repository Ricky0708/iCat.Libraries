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
    public class iCatLocalizerFactory : IiCatLocalizerFactory
    {
        private readonly IiCatStringLocalizer _processor;

        public iCatLocalizerFactory(IiCatStringLocalizer processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public IiCatStringLocalizer Create()
        {
            return _processor;
        }


        public IStringLocalizer Create(Type resourceSource)
        {
            return (IStringLocalizer)Create();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return (IStringLocalizer)Create();
        }
    }
}
