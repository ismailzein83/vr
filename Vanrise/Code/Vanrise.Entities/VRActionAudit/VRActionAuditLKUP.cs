using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRActionAuditLKUP
    {
        public int VRActionAuditLKUPId { get; set; }

        public VRActionAuditLKUPType Type { get; set; }

        public string Name { get; set; }
    }
}
