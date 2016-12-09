using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Business;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class UpdateNotificationStatus : CodeActivity
    {
        public InArgument<Guid> VRNotificationId { get; set; }
        public InArgument<VRNotificationStatus> Status { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            VRNotificationManager manager = new VRNotificationManager();
            manager.UpdateNotificationStatus(VRNotificationId.Get(context), Status.Get(context));
        }
    }
}
