using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{
    public sealed class ExecuteAction : CodeActivity
    {
        public InArgument<IVRActionEventPayload> EventPayload { get; set; }
        public InArgument<VRAction> Action { get; set; }
        public InArgument<int> UserID { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            VRActionExecutionContext vrActionExecutionContext = new VRActionExecutionContext
            {
                EventPayload = EventPayload.Get(context),
                UserID = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId
            };
            Action.Get(context).Execute(vrActionExecutionContext);
        }
    }
}
