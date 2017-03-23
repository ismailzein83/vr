using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRNotificationInput
    {
        public Guid NotificationTypeId { get; set; }
        public long NbOfRows { get; set; }
        public VRNotificationQuery Query { get; set; }
        public VRNotificationExtendedQuery ExtendedQuery { get; set; }
    }
}
