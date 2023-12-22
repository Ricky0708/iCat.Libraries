using Microsoft.VisualStudio.TestTools.UnitTesting;
using iCat.Localization.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Linq;
using iCat.Localization.Interfaces;
using iCat.Localization.Models;

namespace iCat.Localization.Implements.Tests
{
    [TestClass()]
    public class iCatStringLocalizerTests
    {
        #region ILocalizaionProcessor

        [TestMethod()]
        public void SetLanguageCollectionTest()
        {
            // arrange
            var provider = GetProvider();
            var processor = new iCatStringLocalizer(provider);

            // action
            var result_before_changed = processor.Localize("{Name}", "en-US");
            processor.SetLanguageCollection(new Dictionary<string, string> { { "Name", "RRR" } }, "en-US");
            var result_after_changed = processor.Localize("{Name}", "en-US");

            // assert
            Assert.AreEqual("Eric", result_before_changed);
            Assert.AreEqual("RRR", result_after_changed);

        }

        [TestMethod()]
        public void LangAddParamsTest()
        {
            // arrange
            var provider = GetProvider();
            var processor = new iCatStringLocalizer(provider);

            // action
            var result = processor.LangAddParams("Hello, my name is {#Name}", new { Name = "TestUser" });

            // assert
            Assert.AreEqual("##Hello, my name is {#Name}^Name^TestUser@@", result);
        }

        [DataRow("en-US", "TestUser", "My name is TestUser, Age is 18")]
        [DataRow("zh-TW", "測試使用者", "年紀是 18 歲, 測試使用者 是我的名字")]
        [TestMethod()]
        public void SentenceLocalizeTestA(string langCode, string name, string expected)
        {
            // arrange
            var provider = GetProvider();
            var processor = new iCatStringLocalizer(provider);

            // action
            var result = processor.Localize(processor.LangAddParams("{TestSentenceA}", new { Name = name, Age = 18 }), langCode);

            // assert
            Assert.AreEqual(expected, result);
        }

        [DataRow("en-US", "My name is Eric, Age is 18")]
        [DataRow("zh-TW", "年紀是 18 歲, 艾瑞克 是我的名字")]
        [TestMethod()]
        public void SentenceLocalizeTestB(string langCode, string expected)
        {
            // arrange
            var provider = GetProvider();
            var processor = new iCatStringLocalizer(provider);

            // action
            var result = processor.Localize(processor.LangAddParams("{TestSentenceB}", new { Age = 18 }), langCode);

            // assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region IStringLocalizer

        [DataRow("en-US", "Eric")]
        [DataRow("zh-TW", "艾瑞克")]
        [TestMethod()]
        public void LocalizedStringTest(string lang, string expected)
        {
            // arrange
            var provider = GetProvider();
            var processor = new iCatStringLocalizer(provider);
            CultureInfo.CurrentCulture = new CultureInfo(lang);

            // action
            var result = processor["Name"];

            // assert
            Assert.AreEqual(expected, result);
        }

        [DataRow("en-US", "Field Length Overthan 5")]
        [DataRow("zh-TW", "超過 5，欄位Field")]
        [TestMethod()]
        public void LocalizedStringTestStringFormat(string lang, string expected)
        {
            // arrange
            var provider = GetProvider();
            var processor = new iCatStringLocalizer(provider);
            CultureInfo.CurrentCulture = new CultureInfo(lang);

            // action
            var result = processor["Error.MaxLength", "Field", 5];

            // assert
            Assert.AreEqual(expected, result);
        }

        [DataRow("en-US", "TestUser", "My name is TestUser, Age is 18")]
        [DataRow("zh-TW", "測試使用者", "年紀是 18 歲, 測試使用者 是我的名字")]
        [TestMethod()]
        public void LocalizedStringTestWithObject(string lang, string name, string expected)
        {
            // arrange
            var provider = GetProvider();
            var processor = new iCatStringLocalizer(provider);
            CultureInfo.CurrentCulture = new CultureInfo(lang);

            // action
            var result = processor["TestSentenceA", new { Name = name, Age = 18 }];

            // assert
            Assert.AreEqual(expected, result);
        }
   
        #endregion


        private IiCatLocalizationDataProvider GetProvider()
        {
            var provider = Substitute.For<IiCatLocalizationDataProvider>();
            provider.GetLanguageMappingData().Returns(new List<LocalizationMapping> {
                new LocalizationMapping {
                    CultureName = "en-US",
                    LanguageData = new Dictionary<string, string>{
                        {"Error.Required", "Can't be null"},
                        {"Error.MaxLength", "{0} Length Overthan {1}"},
                        {"Name", "Eric"},
                        {"TestSentenceA", "My name is {#Name}, Age is {#Age}" },
                        {"TestSentenceB", "My name is {Name}, Age is {#Age}" },
                    }
                },
                new LocalizationMapping {
                    CultureName = "zh-TW",
                    LanguageData = new Dictionary<string, string>{
                        {"Error.Required", "不能為空"},
                        {"Error.MaxLength", "超過 {1}，欄位{0}"},
                        {"Name", "艾瑞克"},
                        {"TestSentenceA","年紀是 {#Age} 歲, {#Name} 是我的名字" },
                        {"TestSentenceB","年紀是 {#Age} 歲, {Name} 是我的名字" },
                    }
                }
            });
            return provider;
        }
    }
}