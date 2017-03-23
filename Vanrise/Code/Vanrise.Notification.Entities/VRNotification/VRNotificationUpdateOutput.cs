using System.Collections.Generic;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationUpdateOutput
    {
        public List<VRNotificationDetail> VRNotificationDetails { get; set; }

        public byte[] MaxTimeStamp { get; set; }
    }
}
