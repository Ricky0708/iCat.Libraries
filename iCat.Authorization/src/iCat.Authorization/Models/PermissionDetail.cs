using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCat.Authorization.Models
{
    public class PermissionDetail
    {
        public required string PermissionName { get; set; }
        public int Permission { get; set; }
    }
}
