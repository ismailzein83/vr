using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRObjectTracking
    {
        public long VRObjectTrackingId { get; set; }

        public int UserId { get; set; }

        public int LoggableEntityId { get; set; }

        public string ObjectId { get; set; }
    }
}
