using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class TenantType
    {
        public Guid TenantTypeId { get; set; }

        public string Name { get; set; }

        public TenantTypeSettings Settings { get; set; }
    }
}
