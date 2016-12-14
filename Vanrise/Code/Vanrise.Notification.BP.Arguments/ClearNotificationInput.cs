using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Arguments
{
    public class ClearNotificationInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Clear Notification Process";
        }

        public Guid NotificationTypeId {get;set;}

        public VRNotificationParentTypes ParentTypes {get;set;}

        public string EventKey { get; set; }
    }
}
