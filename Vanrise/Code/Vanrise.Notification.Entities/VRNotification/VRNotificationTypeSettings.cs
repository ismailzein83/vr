using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public abstract class VRNotificationTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("FDD73530-067F-4160-AB71-7852303C785C"); }
        }

        public abstract bool CanExecuteNotification(IVRNotificationTypeCanExecuteNotificationContext context);
    }

    public interface IVRNotificationTypeCanExecuteNotificationContext
    {
        string EventKey { get; }
    }
}
