using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRActionAudit
    {
        public long VRActionAuditId { get; set; }

        public int? UserId { get; set; }

        public int UrlId { get; set; }

        public int ModuleId { get; set; }

        public int EntityId { get; set; }

        public int ActionId { get; set; }

        public string ObjectId { get; set; }

        public string ActionDescription { get; set; }

        public DateTime LogTime { get; set; }
    }
}
