using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.BP.Arguments
{
    public interface INotificationProcessArgument
    {
        Guid NotificationTypeId { get; set; }

        string EntityId { get; set; }

        string EventKey { get; set; }
    }
}
