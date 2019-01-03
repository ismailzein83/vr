using System.Activities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public sealed class GetDefinitionObjectState<T> : CodeActivity<T>
    {
        public InArgument<string> ObjectKey { get; set; }
        
        protected override T Execute(CodeActivityContext context)
        {
            var sharedData = context.GetSharedInstanceData();
            BPDefinitionStateManager bpDefinitionStateManager = new BPDefinitionStateManager();
            return bpDefinitionStateManager.GetDefinitionObjectState<T>(sharedData.InstanceInfo.DefinitionID, this.ObjectKey.Get(context));
        }
    }
}