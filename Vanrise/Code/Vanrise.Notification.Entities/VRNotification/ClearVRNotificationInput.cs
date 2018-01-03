using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class ClearVRNotificationInput
    {
        public int UserId { get; set; }
        public Guid NotificationTypeId { get; set; }
        public VRNotificationParentTypes ParentTypes { get; set; }
        public string EventKey { get; set; }

        public string Description { get; set; }

        public string EntityId { get; set; }
    }
}
