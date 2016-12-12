using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class UserActionAuditQuery
    {
        public List<int> UserIds { get; set; }

        public string Module { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string BaseUrl { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        public int TopRecord { get; set; }

    }
}
