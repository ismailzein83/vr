using System;
using System.Activities;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class GetVRNotification : CodeActivity
    {
        [RequiredArgument]
        public InArgument<long> VRNotificationId { get; set; }
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
