using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class BusinessEntityModule
    {
        public Guid ModuleId { get; set; }

        public string Name { get; set; }

     //   public string Title { get; set; }

        public Guid? ParentId { get; set; }
        
        public bool BreakInheritance { get; set; }

        public List<string> PermissionOptions { get; set; }
    }
}
