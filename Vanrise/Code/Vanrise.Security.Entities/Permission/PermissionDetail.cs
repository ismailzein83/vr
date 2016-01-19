using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class PermissionDetail
    {
        public Permission Entity { get; set; }
        public string HolderName { get; set; }
        public string EntityName { get; set; }
    }
}
