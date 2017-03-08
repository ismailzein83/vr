using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRActionAuditDetail
    {
        public VRActionAudit Entity { get; set; }
        public string UserName { get; set; }

        public string ModuleName { get; set; }
        public string EntityName { get; set; }
        public string ActionName { get; set; }
        public string URLName { get; set; }

    }
}
