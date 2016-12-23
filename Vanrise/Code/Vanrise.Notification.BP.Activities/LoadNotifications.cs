using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Notification.Data;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class LoadNotifications : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> NotificationTypeId { get; set; }

        [RequiredArgument]
        public InArgument<VRNotificationParentTypes> ParentTypes { get; set; }

        [RequiredArgument]
        public InArgument<string> EventKey { get; set; }

        [RequiredArgument]
        public OutArgument<List<VRNotification>> Notifications { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IVRNotificationDataManager dataManager = NotificationDataManagerFactory.GetDataManager<IVRNotificationDataManager>();
            Notifications.Set(context, dataManager.GetVRNotifications(NotificationTypeId.Get(context),
                                                                      ParentTypes.Get(context),
                                                                      EventKey.Get(context)));
        }
    }
}
