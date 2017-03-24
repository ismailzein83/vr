using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationFirstPageContext : IGetVRNotificationContext
    {
        Func<VRNotification, bool> onItemReady { get; }

        byte[] MaxTimeStamp { set; }
    }

    public class VRNotificationFirstPageContext : IVRNotificationFirstPageContext
    {
        public Guid NotificationTypeId { get; set; }

        public long NbOfRows { get; set; }

        public VRNotificationQuery Query { get; set; }

        public Func<VRNotification, bool> onItemReady { get; set; }

        public byte[] MaxTimeStamp { get; set; }
    }
}
