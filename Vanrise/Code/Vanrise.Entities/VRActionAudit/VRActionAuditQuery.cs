using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRActionAuditQuery
    {
        public List<int> UserIds { get; set; }

        public List<int> ModuleIds { get; set; }

        public List<int> EntityIds { get; set; }

        public List<int> ActionIds { get; set; }

        public string ObjectId { get; set; }

        public string ObjectName { get; set; }
        public DateTime FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        public int TopRecord { get; set; }

    }

}
