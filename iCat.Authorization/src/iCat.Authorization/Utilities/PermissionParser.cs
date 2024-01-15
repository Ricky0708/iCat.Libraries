using iCat.Authorization.Constants;
using iCat.Authorization.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <summary>
    /// FunctionPermission enum type parser
    /// </summary>
    internal sealed class PermissionParser
    {


        ///// <summary>
        ///// FunctionPermission enum type parser
        ///// </summary>
        ///// <param name="functionEnum"></param>
        ///// <exception cref="ArgumentException"></exception>
        //public PermissionParser(Type functionEnum)
        //{
        //    _functionDatas ??= GetDefinitions(functionEnum, GetPermissionEnumList(functionEnum));
        //}

        /////// <summary>
        /////// FunctionPermission enum type parser
        /////// </summary>
        /////// <param name="functionEnum"></param>
        /////// <param name="functionPermissionEnums"></param>
        /////// <exception cref="ArgumentException"></exception>
        ////public FunctionPermissionParser(Type functionEnum, params Type[] functionPermissionEnums)
        ////{
        ////    if (!CheckNamingDefinition(functionEnum, functionPermissionEnums)) throw new ArgumentException("Needs to be an enum type and must follow naming rules. (the name remove suffix from functionPermission type needs to match function type name)");
        ////    _functionDatas ??= GetPermissionDefinitions(functionEnum, functionPermissionEnums);
        ////}


        //#region private methods





        ///// <summary>
        ///// Check type and naming rule
        ///// </summary>
        ///// <param name="functionEnum"></param>
        ///// <param name="functionPermissionEnums"></param>
        ///// <returns></returns>
        //private bool CheckNamingDefinition(Type functionEnum, params Type[] functionPermissionEnums)
        //{
        //    var result = true;
        //    if (result) result = functionEnum.IsEnum && functionPermissionEnums.All(p => p.IsEnum);
        //    if (result) result = functionPermissionEnums.All(p => Enum.GetNames(functionEnum).Contains(p.Name.Replace(_endWith, "")));
        //    if (result) result = Enum.GetNames(functionEnum).All(p => functionPermissionEnums.Any(d => d.Name.Replace(_endWith, "") == p));
        //    return result;
        //}

        //#endregion
    }
}
