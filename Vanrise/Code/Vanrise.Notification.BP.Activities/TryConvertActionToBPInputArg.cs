using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Notification.Business;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class TryConvertActionToBPInputArg : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CreateVRActionInput> CreateVRActionInput { get; set; }

        [RequiredArgument]
        public OutArgument<Vanrise.BusinessProcess.Entities.BaseProcessInputArgument> BPInputArgument { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var createVRActionInput = CreateVRActionInput.Get(context);

            VRActionConvertToBPInputArgumentContext vrActionConvertContext = new VRActionConvertToBPInputArgumentContext
            {
                EventPayload = createVRActionInput.EventPayload                
            };
            if (createVRActionInput.Action.TryConvertToBPInputArgument(vrActionConvertContext))
                this.BPInputArgument.Set(context, vrActionConvertContext.BPInputArgument);
        }
    }
}
