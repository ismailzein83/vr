using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRPushNotificationHandlerSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IVRPushNotificationHandlerExecuteContext context);
    }

    public interface IVRPushNotificationHandlerExecuteContext
    {
        List<IVRPushNotificationSubscription> Subscriptions { get; }

        void SendMessage(VRPushNotificationMessage message, IVRPushNotificationSubscription subscription);

        Object HandlerState { get; set; }
    }
    
    public class VRBPMyTasksCountPushNotificationHandler : VRPushNotificationHandlerSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("85AE8D6D-411B-44B3-B227-A33D8566E926"); }
        }

        List<BPTask> _tasks;

        Object _lastUpdatedTaskTimeStamp;
        int taskCount;
        public override void Execute(IVRPushNotificationHandlerExecuteContext context)
        {
            foreach(var subscription in context.Subscriptions)
            {
                context.SendMessage(new VRBPMyTasksCountPushNotificationMessage { Count = ++taskCount, SubscriptionCount = context.Subscriptions.Count }, subscription);
            }
        }
    }

    public class VRBPMyTasksCountPushNotificationMessage : VRPushNotificationMessage
    {
        public int Count { get; set; }

        public int SubscriptionCount { get; set; }
    }

    public class BPTask
    {

    }

    public class VRBPMyTasksCountPushNotificationSubscription : VRPushNotificationClientSubscription
    {
        public override Guid HandlerId
        {
            get { return new Guid("85AE8D6D-411B-44B3-B227-A33D8566E926"); }
        }
    }

}
