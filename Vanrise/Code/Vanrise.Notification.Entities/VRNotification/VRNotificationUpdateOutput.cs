using System.Collections.Generic;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationUpdateOutput
    {
        public List<VRNotificationDetail> VRNotificationDetails { get; set; }

        public object LastUpdateHandle { get; set; }
    }
}
