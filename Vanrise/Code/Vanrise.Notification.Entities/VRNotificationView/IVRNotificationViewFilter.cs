using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public interface IVRNotificationViewFilter
    {
        bool IsMatched(IVRNotificationViewFilterContext context);
    }

    public interface IVRNotificationViewFilterContext
    {
        Guid VRNotificationTypeId { get; }
    }
}
