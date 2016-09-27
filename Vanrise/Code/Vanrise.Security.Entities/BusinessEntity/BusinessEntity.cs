using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class BusinessEntity
    {
        public Guid EntityId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public Guid ModuleId { get; set; }

        public bool BreakInheritance { get; set; }

        public List<string> PermissionOptions { get; set; }
    }

    public class BusinessEntityInfo
    {
        public Guid EntityId { get; set; }

        public string Name { get; set; }

    }
}
