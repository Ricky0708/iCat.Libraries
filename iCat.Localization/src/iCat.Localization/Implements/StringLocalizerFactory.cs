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
    /// <summary>
    /// StringLocalizerFactory
    /// </summary>
    public class StringLocalizerFactory : Interfaces.IStringLocalizerFactory
    {
        private readonly Interfaces.IStringLocalizer _processor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StringLocalizerFactory(Interfaces.IStringLocalizer processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public Interfaces.IStringLocalizer Create()
        {
            return _processor;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="resourceSource"></param>
        /// <returns></returns>
        public Microsoft.Extensions.Localization.IStringLocalizer Create(Type resourceSource)
        {
            return (Microsoft.Extensions.Localization.IStringLocalizer)Create();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public Microsoft.Extensions.Localization.IStringLocalizer Create(string baseName, string location)
        {
            return (Microsoft.Extensions.Localization.IStringLocalizer)Create();
        }
    }
}
