using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Data;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class GetVRNotification : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> VRNotificationId { get; set; }
        [RequiredArgument]
        public OutArgument<VRNotification> VRNotification { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            VRNotification notification = dataManager.GetVRNotificationById(VRNotificationId.Get(context));
            if (notification == null)
                throw new NullReferenceException("notification");
            if (notification.Data == null)
                throw new NullReferenceException("notification.Data");

            VRNotification.Set(context, notification);
        }
    }
}
