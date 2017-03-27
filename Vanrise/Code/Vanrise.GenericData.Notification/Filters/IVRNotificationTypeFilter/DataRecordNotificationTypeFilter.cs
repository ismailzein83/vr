using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordNotificationTypeFilter : IVRNotificationTypeFilter
    {
        public Guid DataRecordTypeId { get; set; }

        public bool IsMatched(IVRNotificationTypeFilterContext context)
        {
            if (context.VRNotificationType.Settings == null)
                return false;

            VRNotificationTypeSettings vrNotificationTypeSettings = context.VRNotificationType.Settings as VRNotificationTypeSettings;
            if (vrNotificationTypeSettings == null)
                return false;

            if (vrNotificationTypeSettings.ExtendedSettings == null)
                return false;

            DataRecordNotificationTypeSettings dataRecordNotificationTypeSettings = vrNotificationTypeSettings.ExtendedSettings as DataRecordNotificationTypeSettings;
            if (dataRecordNotificationTypeSettings == null)
                return false;

            if (dataRecordNotificationTypeSettings.DataRecordTypeId != this.DataRecordTypeId)
                return false;

            return true;
        }
    }
}
