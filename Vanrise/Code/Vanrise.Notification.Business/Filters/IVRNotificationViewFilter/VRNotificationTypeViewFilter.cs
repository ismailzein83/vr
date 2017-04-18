using System;
using System.Linq;
using Vanrise.Notification.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Business
{
    public class VRNotificationTypeViewFilter : IVRNotificationTypeFilter
    {
        public Guid ViewId { get; set; }
        public bool IsMatched(IVRNotificationTypeFilterContext context)
        {
            if (this.ViewId != Guid.Empty)
            {
                ViewManager viewManager = new ViewManager();

                View vrNotificationView = viewManager.GetView(this.ViewId);
                var settings = vrNotificationView.Settings as VRNotificationViewSettings;
                var vrNotificationTypeManager = new VRNotificationTypeManager();
                if (!vrNotificationTypeManager.DoesUserHaveViewAccess(ContextFactory.GetContext().GetLoggedInUserId(), context.VRNotificationType.Settings))
                    return false;
                if (!settings.Settings.Any(s => s.VRNotificationTypeId == context.VRNotificationType.VRComponentTypeId))
                    return false;
            }
            return true;
        }
    }
}