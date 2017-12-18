using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRExclusiveSessionDetail
    {
        public int VRExclusiveSessionID { get; set; }

        public Guid SessionTypeId { get; set; }

        public string SessionType { get; set; }

        public string TargetId { get; set; }

        public string TargetName { get; set; }

        public int TakenByUserId { get; set; }      

        public int LockedByUser { get; set; }

        public DateTime LastTakenUpdateTime { get; set;  }

        public DateTime CreatedTime { get; set; }
    }
}
