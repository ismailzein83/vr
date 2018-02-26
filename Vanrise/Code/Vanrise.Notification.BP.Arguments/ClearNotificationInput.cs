using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Arguments
{
    public class ClearNotificationInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument, INotificationProcessArgument
    {
        public override string GetTitle()
        {
            return this.ProcessTitle != null ? this.ProcessTitle : "Clear Notification Process";
        }

        public string ProcessTitle { get; set; }

        public Guid NotificationTypeId {get;set;}

        public IVRActionRollbackEventPayload RollbackEventPayload { get; set; }

        public VRNotificationParentTypes ParentTypes {get;set;}

        public string EventKey { get; set; }
    }
}
