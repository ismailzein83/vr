using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationTypeSettings : VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("FDD73530-067F-4160-AB71-7852303C785C"); }
        }

        public VRNotificationTypeExtendedSettings ExtendedSettings { get; set; }   

        //public abstract bool CanExecuteNotification(IVRNotificationTypeCanExecuteNotificationContext context);
    }

    public interface IVRNotificationTypeCanExecuteNotificationContext
    {
        string EventKey { get; }
    }
}
