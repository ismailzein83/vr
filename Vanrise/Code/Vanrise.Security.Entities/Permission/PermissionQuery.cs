using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class PermissionQuery
    {
        public string EntityId { get; set; }
        public EntityType EntityType { get; set; }
    }
}
