using iCat.Localization.Extensions;
using iCat.Localization.Interfaces;
using iCat.Localization.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iCat.Localization.Implements
{
    /// <summary>
    /// Localizaion Processor
    /// </summary>
    public class StringLocalizer : Interfaces.IStringLocalizer
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Languages => _langCache;

        private delegate string delgGetData(object instance);
        private readonly Dictionary<string, Dictionary<string, string>> _langCache = new Dictionary<string, Dictionary<string, string>>();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _processedCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        private readonly ConcurrentDictionary<string, delgGetData> _delgCacheGetParam = new ConcurrentDictionary<string, delgGetData>();
        private readonly ConcurrentDictionary<string, delgGetData> _delgCacheGetProperty = new ConcurrentDictionary<string, delgGetData>();
        private readonly Regex _reg = new Regex("(##)[^###](?!.*\\1).+?@@", RegexOptions.Multiline | RegexOptions.Singleline);
        private readonly IStringLocalizationDataProvider _localizationDataProvider;
        private readonly Options _options;

        public StringLocalizer(IStringLocalizationDataProvider localizationDataProvider, Options? options = null)
        {
            _localizationDataProvider = localizationDataProvider ?? throw new ArgumentNullException(nameof(localizationDataProvider));
            _options = options ?? new Options();
            _localizationDataProvider.NotifyUpdate += UpdateCache;
            UpdateCache();
        }

        #region ILocalizaionProcessor

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="langCode"></param>
        public void SetLanguageCollection(Dictionary<string, string> dic, string langCode)
        {

            if (!_langCache.ContainsKey(langCode))
            {
                lock (_langCache)
                {
                    if (!_langCache.ContainsKey(langCode))
                    {
                        _langCache.Add(langCode, dic);
                    }
                }
            }
            if (!_processedCache.ContainsKey(langCode))
            {
                lock (_processedCache)
                {
                    if (!_processedCache.ContainsKey(langCode))
                    {
                        _processedCache.TryAdd(langCode, new ConcurrentDictionary<string, string>());
                    }
                }
            }

            _langCache[langCode] = dic;
            lock (_processedCache[langCode])
            {
                _processedCache[langCode].Clear();
            }
            lock (_delgCacheGetParam)
            {
                _delgCacheGetParam.Clear();
            }
            lock (_delgCacheGetProperty)
            {
                _delgCacheGetProperty.Clear();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public string AddParams(string str, object paramData)
        {
            var cacheKey = $"{str}{paramData.GetType().ToString()}";
            if (!_delgCacheGetProperty.TryGetValue(cacheKey, out var lambda))
            {
                lock (_delgCacheGetProperty)
                {
                    if (!_delgCacheGetProperty.TryGetValue(cacheKey, out lambda))
                    {
                        var exprList = new List<Expression>();
                        var targetExpr = Expression.Parameter(typeof(object), "target");
                        var memberExpr = Expression.Convert(targetExpr, paramData.GetType());
                        var props = paramData.GetType().GetProperties();
                        exprList.Add(Expression.Constant(str));
                        exprList.Add(Expression.Constant("^"));
                        foreach (var prop in props)
                        {
                            var propExpression = Expression.Property(memberExpr, prop);
                            var toStringExpression = default(MethodCallExpression);
                            if (propExpression.Type != typeof(string))
                            {
                                var toString = propExpression.Type.GetMethods().First(p => p.Name.ToLower() == "tostring");
                                toStringExpression = Expression.Call(propExpression, toString);
                            }
                            var constExpression = Expression.Constant(propExpression.Member.Name);
                            exprList.Add(constExpression);
                            exprList.Add(Expression.Constant("^"));
                            exprList.Add(toStringExpression != null ? toStringExpression as Expression : propExpression);
                            exprList.Add(Expression.Constant("^"));
                        }
                        exprList.RemoveAt(exprList.Count - 1);
                        var method = typeof(string).GetMethod("Concat", new[] { typeof(object[]) });
                        var paramsExpr = Expression.NewArrayInit(typeof(object), exprList);
                        var methodExpr = Expression.Call(method!, paramsExpr);
                        var lambdaExpr = Expression.Lambda<delgGetData>(methodExpr, targetExpr);
                        lambda = lambdaExpr.Compile();
                        _delgCacheGetProperty.TryAdd(cacheKey, lambda);
                    }
                }
            }

            return $"##{lambda.Invoke(paramData)}@@";

            //var param = JsonConvert.SerializeObject(new { Key = str.Replace("{", "").Replace("}", ""), Data = paramData });
            //return $"##{param}";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public string Localize(string str, string lang)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return str;
                var paramModel = default(Dictionary<string, string>);
                var match = default(Match);
                var isSharpSharp = false;
                while ((match = _reg.Match(str)).Length != 0)
                {
                    isSharpSharp = true;
                    var obj = match.Value.Substring(2, match.Value.Length - 4).Split('^');
                    var tempResult = obj[0];
                    paramModel = new Dictionary<string, string>();
                    for (int i = 1; i < obj.Length - 1; i += 2)
                    {
                        paramModel.Add(obj[i], obj[i + 1]);
                    }
                    str = _reg.Replace(str, Parser(tempResult, lang, paramModel));
                }
                if (isSharpSharp) return Localize(str, lang);

                if (!_processedCache[lang].TryGetValue(str, out var result))
                {
                    lock (_processedCache[lang])
                    {
                        if (!_processedCache[lang].TryGetValue(str, out result))
                        {
                            result = Parser(str, lang, paramModel!);
                            _processedCache[lang].TryAdd(str, result);
                        }
                    }
                }

                return result;
            }
            catch (KeyNotFoundException ex)
            {
                if (_options.EnableKeyNotFoundException)
                {
                    throw new KeyNotFoundException($"Localization Key:{ex.Message} is not found in {lang}");
                }
                else
                {
                    return str;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region IStringLocalizer

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var result = $"{{{name}}}";
                foreach (var item in arguments)
                {
                    if (!item.GetType().IsValueType && item.GetType() != typeof(string))
                    {
                        result = AddParams(result, item);
                        result = Localize(result, CultureInfo.CurrentCulture.Name);
                    }
                    else
                    {
                        var format = _langCache[CultureInfo.CurrentCulture.Name].ContainsKey(name) ? _langCache[CultureInfo.CurrentCulture.Name][name] : name;
                        result = string.Format(format ?? name, arguments);
                    }
                }
                return new LocalizedString(name, result, resourceNotFound: false);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LocalizedString this[string name]
        {
            get
            {
                var value = Localize($"{{{name}}}", CultureInfo.CurrentCulture.Name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="includeParentCultures"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _langCache
                .First(l => l.Key == CultureInfo.CurrentCulture.Name).Value
                .Select(l => new LocalizedString(l.Key, l.Value, false));
        }

        #endregion

        #region private methods

        /// <summary>
        /// renew localization data cache
        /// </summary>
        private void UpdateCache()
        {
            foreach (var item in _localizationDataProvider.GetLanguageMappingData())
            {
                SetLanguageCollection(item.LanguageData!, item.CultureName!);
            }
        }

        /// <summary>
        /// string parser
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lang"></param>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        private string Parser(string str, string lang, object paramModel)
        {
            var sb = new StringBuilder();
            var start = false;
            var key = new StringBuilder();
            var isParam = false;
            var result = "";
            try
            {
                foreach (var chr in str)
                {
                    if (chr == '{')
                    {
                        start = true;
                    }
                    else if (chr == '}')
                    {
                        if (isParam)
                        {
                            sb.Append(Parser(ProcessParam(key.ToString(), paramModel).Invoke(paramModel), lang, paramModel));
                        }
                        else
                        {
                            sb.Append(Parser(_langCache[lang][key.ToString()], lang, paramModel));
                        }
                        key.Clear();
                        start = false;
                        isParam = false;
                    }
                    else if (start && chr == '#')
                    {
                        isParam = true;
                    }
                    else
                    {
                        if (start)
                        {
                            key.Append(chr);
                        }
                        else
                        {
                            sb.Append(chr);
                        }
                    }

                }
            }
            catch (KeyNotFoundException kex)
            {
                //var ex = new KeyNotFoundException(kex.Message, new KeyNotFoundException($"{key.ToString()}{(kex.InnerException == null ? null : "→" + kex.InnerException.Message)}"));
                if (_options.EnableKeyNotFoundException)
                {
                    var ex = new KeyNotFoundException($"{key.ToString()}{(kex.InnerException == null ? null : "→" + kex.Message)}", kex);
                    throw ex;
                }
                else
                {
                    return str;
                }

            }
            catch (Exception)
            {
                throw;
            }

            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// process dynamic parameters
        /// </summary>
        /// <param name="str"></param>
        /// <param name="paramModel"></param>
        /// <returns></returns>
        private delgGetData ProcessParam(string str, object paramModel)
        {
            if (!_delgCacheGetParam.TryGetValue(str, out var lambda))
            {
                lock (_delgCacheGetParam)
                {
                    if (!_delgCacheGetParam.TryGetValue(str, out lambda))
                    {
                        var sb = new StringBuilder();
                        var key = new StringBuilder();
                        //var exprList = new List<Expression>();
                        var targetExpr = Expression.Parameter(typeof(object), "target");
                        var memberExpr = Expression.Convert(targetExpr, paramModel.GetType());

                        var keyExpr = Expression.Constant(str);
                        PropertyInfo indexer = (from p in memberExpr.Type.GetDefaultMembers().OfType<PropertyInfo>()
                                                where p.PropertyType == typeof(string)
                                                let q = p.GetIndexParameters()
                                                where q.Length == 1 && q[0].ParameterType == typeof(string)
                                                select p).Single();
                        IndexExpression indexExpr = Expression.Property(memberExpr, indexer, keyExpr);
                        //exprList.Add(indexExpr);

                        key.Clear();
                        sb.Clear();

                        //var method = typeof(string).GetMethod("Concat", new[] { typeof(object[]) });
                        //var paramsExpr = Expression.NewArrayInit(typeof(object), exprList);
                        //var methodExpr = Expression.Call(method, paramsExpr);
                        var lambdaExpr = Expression.Lambda<delgGetData>(indexExpr, targetExpr);
                        lambda = lambdaExpr.Compile();
                        _delgCacheGetParam.TryAdd(str, lambda);
                    }
                }
            }

            return lambda;
        }

        #endregion
    }
}
