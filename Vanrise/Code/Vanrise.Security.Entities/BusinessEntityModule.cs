using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class BusinessEntityModule
    {
        public int ModuleId { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public List<string> PermissionOptions { get; set; }
    }
}
