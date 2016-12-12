using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class UserActionAudit
    {
        public long UserActionAuditId { get; set; }

        public int? UserId { get; set; }

        public string ModuleName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string BaseUrl { get; set; }

        public DateTime LogTime { get; set; }

    }
}
