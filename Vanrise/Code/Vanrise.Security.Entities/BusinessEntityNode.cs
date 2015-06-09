using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class BusinessEntityNode
    {
        public int EntityId { get; set; }

        public string Name { get; set; }

        public EntityType EntType { get; set; }

        public List<string> PermissionOptions { get; set; }

        public List<BusinessEntityNode> Children { get; set; }

    }

    public enum EntityType
    {
        MODULE,
        ENTITY
    }
}
