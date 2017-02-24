using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Notification.Business
{
    public class VRNotificationTypeViewFilter : IVRNotificationViewFilter
    {
        public Guid ViewId { get; set; }
        public bool IsMatched(IVRNotificationViewFilterContext context)
        {
            if (this.ViewId != Guid.Empty)
            {
                ViewManager viewManager = new ViewManager();

                View vrNotificationView = viewManager.GetView(this.ViewId);
                var settings = vrNotificationView.Settings as VRNotificationViewSettings;

                if (!settings.Settings.Any(s => s.VRNotificationTypeId == context.VRNotificationTypeId))
                    return false;
            }
            return true;
        }
    }
}
