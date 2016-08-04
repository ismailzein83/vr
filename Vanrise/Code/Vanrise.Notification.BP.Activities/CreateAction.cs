using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Business;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class CreateAction : CodeActivity
    {
        public InArgument<CreateVRActionInput> CreateVRActionInput { get; set; }
        public InArgument<int> UserID { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var createVRActionInput = CreateVRActionInput.Get(context);
            VRActionExecutionContext vrActionExecutionContext = new VRActionExecutionContext{
                EventPayload = createVRActionInput.EventPayload,
                UserID = UserID.Get(context)
            };
            createVRActionInput.Action.Execute(vrActionExecutionContext);
        }
    }
}
