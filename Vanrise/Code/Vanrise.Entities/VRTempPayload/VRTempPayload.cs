using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRTempPayload
    {
        public Guid VRTempPayloadId { get; set; }
        public VRTempPayloadSettings Settings { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
    }

    public abstract class VRTempPayloadSettings
    {
    }

}
