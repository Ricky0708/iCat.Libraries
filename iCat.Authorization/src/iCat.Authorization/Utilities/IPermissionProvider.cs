﻿using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Utilities
{
    /// <summary>
    /// Provide user's permission
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// Get AuthorizationPermissin attribute information
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        List<Function> GetPermissionRequired(params CustomAttributeData[] attributes);

        /// <summary>
        /// Get function and permission mapping
        /// </summary>
        /// <returns></returns>
        List<Function> GetDefinitions();
    }
}