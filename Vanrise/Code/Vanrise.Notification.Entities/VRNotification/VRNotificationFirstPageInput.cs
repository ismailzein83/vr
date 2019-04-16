using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationFirstPageInput : VRNotificationInput
    {

    }

    public class VRNotificationExportInput
    {
        public Guid NotificationTypeId { get; set; }
        public VRNotificationExportQuery Query { get; set; }
        public VRNotificationExtendedQuery ExtendedQuery { get; set; }
    }
}