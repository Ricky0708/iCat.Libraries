using iCat.Token.Interfaces;
using iCat.Token.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Token.Implements
{
    public class TokenService<T> : ITokenService<T>
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ITokenValidator _tokenValidator;
        private static readonly ConcurrentDictionary<string, delgGetPropString> _dicGetString = new ConcurrentDictionary<string, delgGetPropString>();
        private static readonly ConcurrentDictionary<string, delgGetPropLong> _dicGetLong = new ConcurrentDictionary<string, delgGetPropLong>();
        private static readonly ConcurrentDictionary<string, delgGetPropIEnumerable> _dicGetList = new ConcurrentDictionary<string, delgGetPropIEnumerable>();


        private static readonly ConcurrentDictionary<string, delgSetPropS> _dicSetProp = new ConcurrentDictionary<string, delgSetPropS>();
        private static readonly ConcurrentDictionary<string, delgAddPropList> _dicAddList = new ConcurrentDictionary<string, delgAddPropList>();


        private delegate string delgGetPropString(T obj);
        private delegate long delgGetPropLong(T obj);
        private delegate IEnumerable delgGetPropIEnumerable(T obj);


        private delegate void delgSetPropS(T obj, object value);
        private delegate void delgAddPropList(T obj, object value);


        public TokenService(ITokenGenerator tokenGenerator, ITokenValidator tokenValidator)
        {
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
            _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        }

        public string GenerateToken(T dataModel)
        {
            var props = typeof(T).GetProperties();
            var claims = new List<Claim>();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    var result = GetGetDelg(_dicGetString, prop).Invoke(dataModel);
                    claims.Add(new Claim(prop.Name, result));
                }
                else if (prop.PropertyType.IsEnum)
                {
                    var result = GetGetDelg(_dicGetLong, prop).Invoke(dataModel);
                    claims.Add(new Claim(prop.Name, result.ToString()));
                }
                else if (prop.PropertyType.IsArray ||
                    prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var result = GetGetDelg(_dicGetList, prop).Invoke(dataModel);
                    foreach (var item in result)
                    {
                        claims.Add(new Claim(prop.Name, item.GetType().IsEnum
                            ? Convert.ToInt64(item).ToString()!
                            : item.ToString()!));
                    }
                }
                else if (
                    prop.PropertyType == typeof(byte) || prop.PropertyType == typeof(short) ||
                    prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long)
                    )
                {
                    var result = GetGetDelg(_dicGetLong, prop).Invoke(dataModel);
                    claims.Add(new Claim(prop.Name, result.ToString()));
                }
            }
            return GenerateToken(claims);
        }

        public string GenerateToken(List<Claim> claims)
        {
            return _tokenGenerator.GenerateToken(claims);
        }

        public ValidationResult ValidateToken(string token)
        {
            var result = _tokenValidator.Validate(token);
            return result;
        }

        public ValidationDataResult<T> ValidateWithReturnData(string token)
        {
            var validateResult = _tokenValidator.Validate(token);
            var result = new ValidationDataResult<T>()
            {
                IsValid = validateResult.IsValid,
                ErrorMsg = validateResult.ErrorMsg,
                TokenData = default(T)
            };
            if (validateResult.IsValid)
            {
                result.TokenData = Activator.CreateInstance<T>();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    var values = validateResult.Principal!.Claims.Where(p => p.Type == prop.Name).Select(p => p.Value).ToArray();
                    AssignData(result.TokenData, prop, values);
                }
                return result;
            }
            return result;
        }

        private void AssignData(T tokenData, PropertyInfo prop, string[] values)
        {
            if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
            {
                var value = values.First();
                GetSetDelg(_dicSetProp, prop).Invoke(tokenData, value);
            }
            else if (prop.PropertyType.IsArray ||
                prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (GetGetDelg(_dicGetList, prop).Invoke(tokenData) == null)
                {
                    var instance = Activator.CreateInstance(prop.PropertyType)!;
                    GetSetDelg(_dicSetProp, prop).Invoke(tokenData, instance);
                }

                foreach (var value in values)
                {
                    var n = GetAddDelg(_dicAddList, prop);
                    n.Invoke(tokenData, value);
                }
            }
        }

        private V GetGetDelg<V>(ConcurrentDictionary<string, V> dic, PropertyInfo prop)
        {
            var key = $"{typeof(T).Name}.{prop.Name}";
            if (!dic.TryGetValue(key, out var delg))
            {
                lock (dic)
                {
                    if (!dic.TryGetValue($"{typeof(T).Name}.{prop.Name}", out delg))
                    {
                        var targetExpr = Expression.Parameter(typeof(T), "target");
                        var memExpr = Expression.Property(targetExpr, prop);
                        var lambdax = Expression.Lambda<V>(
                            Expression.Convert(memExpr, dic.GetType().GenericTypeArguments[1].GetMethod("Invoke")!.ReturnType),
                            targetExpr);
                        delg = lambdax.Compile();
                        dic.TryAdd(key, delg);
                    }
                }
            }
            return delg;
        }

        private V GetSetDelg<V>(ConcurrentDictionary<string, V> dic, PropertyInfo prop)
        {
            var key = $"{typeof(T).Name}.{prop.Name}";
            if (!dic.TryGetValue(key, out var delg))
            {
                lock (dic)
                {
                    if (!dic.TryGetValue($"{typeof(T).Name}.{prop.Name}", out delg))
                    {
                        var targetExpr = Expression.Parameter(typeof(T), "target");
                        var valueExpr = Expression.Parameter(typeof(V).GetMethod("Invoke")!.GetParameters()[1].ParameterType, "value");

                        var methodExpr = prop.PropertyType.IsValueType
                            ? prop.PropertyType.IsEnum
                                   ? (Expression)Expression.Convert(Expression.Call(typeof(Convert).GetMethod(GetConvertName(prop.PropertyType), new[] { typeof(object) })!, valueExpr), prop.PropertyType)
                                   : (Expression)Expression.Call(typeof(Convert).GetMethod(GetConvertName(prop.PropertyType), new[] { typeof(object) })!, valueExpr)
                            : (Expression)Expression.Convert(valueExpr, prop.PropertyType);
                        var memExpr = Expression.Property(targetExpr, prop);

                        var assignExpr = Expression.Assign(memExpr, methodExpr);
                        var lambdax = Expression.Lambda<V>(assignExpr, targetExpr, valueExpr);
                        delg = lambdax.Compile();
                        dic.TryAdd(key, delg);
                    }
                }
            }
            return delg;
        }

        private V GetAddDelg<V>(ConcurrentDictionary<string, V> dic, PropertyInfo prop)
        {
            var key = $"{typeof(T).Name}.{prop.Name}";
            if (!dic.TryGetValue(key, out var delg))
            {
                lock (dic)
                {
                    if (!dic.TryGetValue($"{typeof(T).Name}.{prop.Name}", out delg))
                    {
                        var targetExpr = Expression.Parameter(typeof(T), "target");
                        var valueExpr = Expression.Parameter(typeof(V).GetMethod("Invoke")!.GetParameters()[1].ParameterType, "value");
                        //var convertExpr = Expression.Convert(valueExpr, prop.PropertyType.GenericTypeArguments[0]);
                        var methodExpr = prop.PropertyType.GenericTypeArguments[0].IsValueType
                            ? prop.PropertyType.GenericTypeArguments[0].IsEnum
                                   ? (Expression)Expression.Convert(Expression.Call(typeof(Convert).GetMethod(GetConvertName(prop.PropertyType.GenericTypeArguments[0]), new[] { typeof(object) })!, valueExpr), prop.PropertyType.GenericTypeArguments[0])
                                   : (Expression)Expression.Call(typeof(Convert).GetMethod(GetConvertName(prop.PropertyType.GenericTypeArguments[0]), new[] { typeof(object) })!, valueExpr)
                            : (Expression)Expression.Convert(valueExpr, prop.PropertyType.GenericTypeArguments[0]);

                        var memExpr = Expression.Property(targetExpr, prop);
                        var callExpr = Expression.Call(memExpr, prop.PropertyType.GetMethod("Add")!, methodExpr);
                        var lambdax = Expression.Lambda<V>(callExpr, targetExpr, valueExpr);
                        delg = lambdax.Compile();
                        dic.TryAdd(key, delg);
                    }
                }
            }
            return delg;
        }

        private string GetConvertName(Type propType)
        {
            var enumName = propType.IsEnum ? "Int64" : Enum.GetNames(typeof(TypeCode)).FirstOrDefault(p => p == propType.Name);
            var typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), enumName!);

            switch (typeCode)
            {
                case TypeCode.String: return nameof(Convert.ToString);
                case TypeCode.Byte: return nameof(Convert.ToByte);
                case TypeCode.Int16: return nameof(Convert.ToInt16);
                case TypeCode.Int32: return nameof(Convert.ToInt32);
                case TypeCode.Int64: return nameof(Convert.ToInt64);
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                default: throw new NotImplementedException($"{propType.Name} convert is not implemented yet.");
            }
        }
    }
}
