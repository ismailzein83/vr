using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public interface IGetVRNotificationContext
    {
        Guid NotificationTypeId { get; }

        long NbOfRows { get; }

        VRNotificationQuery Query { get; }
    }
}
