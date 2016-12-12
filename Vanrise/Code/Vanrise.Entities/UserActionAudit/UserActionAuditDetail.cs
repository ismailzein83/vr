using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class UserActionAuditDetail
    {
        public UserActionAudit Entity { get; set; }
        public string UserName { get; set; }

    }
}
