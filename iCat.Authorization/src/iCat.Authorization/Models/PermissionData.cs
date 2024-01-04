using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    public class PermissionData
    {
        public required string FunctionName { get; set; }
        public required int FunctionValue { get; set; }
        public required List<PermissionDetail> PermissionDetailList { get; set; }
        public int Permissions => PermissionDetailList.Sum(p => p.Permission);
    }
}
