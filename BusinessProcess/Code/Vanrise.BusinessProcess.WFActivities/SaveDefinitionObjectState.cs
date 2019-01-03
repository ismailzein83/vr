using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class SaveDefinitionObjectState : CodeActivity
    {
        public InArgument<string> ObjectKey { get; set; }

        [RequiredArgument]
        public InArgument<object> ObjectValue { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var sharedData = context.GetSharedInstanceData();
            BPDefinitionStateManager bpDefinitionStateManager = new BPDefinitionStateManager();
            bpDefinitionStateManager.SaveDefinitionObjectState(sharedData.InstanceInfo.DefinitionID, this.ObjectKey.Get(context), this.ObjectValue.Get(context));
        }
    }
}