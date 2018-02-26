using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{
    public sealed class ExecuteAction : Vanrise.BusinessProcess.BaseCodeActivity
    {
        public InArgument<IVRActionEventPayload> EventPayload { get; set; }
        public InArgument<IVRActionRollbackEventPayload> RollbackEventPayload { get; set; }
        public InArgument<VRAction> Action { get; set; }
        public InArgument<int> UserID { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            VRActionExecutionContext vrActionExecutionContext = new VRActionExecutionContext
            {
                EventPayload = EventPayload.Get(context.ActivityContext),
                RollbackEventPayload = this.RollbackEventPayload.Get(context.ActivityContext),
                UserID = context.ActivityContext.GetSharedInstanceData().InstanceInfo.InitiatorUserId
            };
            Action.Get(context.ActivityContext).Execute(vrActionExecutionContext);
        }
    }
}
