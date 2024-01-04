using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    public class FunctionData
    {
        public string? FunctionName { get; set; }
        public int? FunctionValue { get; set; }
        public List<PermissionDetail> PermissionDetails { get; set; } = new List<PermissionDetail>();
        public int Permissions => PermissionDetails?.Sum(p => p.Permission) ?? 0;
    }
}
