using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRExclusiveSession
    {
        public int VRExclusiveSessionID { get; set; }

        public Guid SessionTypeId { get; set; }

        public string TargetId { get; set; }

        public int TakenByUserId { get; set; }

        public DateTime LastTakenUpdateTime { get; set;  }

        public DateTime CreatedTime { get; set; }
    }
}
