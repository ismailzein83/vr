using System.Activities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{
    public sealed class TryConvertActionToBPInputArgumnet : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CreateVRActionInput> CreateVRActionInput { get; set; }

        [RequiredArgument]
        public OutArgument<BaseProcessInputArgument> BPInputArgument { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var createVRActionInput = CreateVRActionInput.Get(context);

            VRActionConvertToBPInputArgumentContext vrActionConvertContext = new VRActionConvertToBPInputArgumentContext
            {
                EventPayload = createVRActionInput.EventPayload,
                RollbackEventPayload = createVRActionInput.RollbackEventPayload
            };
            if (createVRActionInput.Action.TryConvertToBPInputArgument(vrActionConvertContext))
                this.BPInputArgument.Set(context, vrActionConvertContext.BPInputArgument);
        }
    }
}
