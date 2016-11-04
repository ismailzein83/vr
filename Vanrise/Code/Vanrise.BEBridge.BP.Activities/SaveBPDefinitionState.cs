using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;

namespace Vanrise.BEBridge.BP.Activities
{
    public class SaveBPDefinitionStateInput
    {
        public Guid BeDefinitionId { get; set; }
        public object ReaderState { get; set; }
    }

    public sealed class SaveBPDefinitionState : DependentAsyncActivity<SaveBPDefinitionStateInput>
    {
        [RequiredArgument]
        public InArgument<Guid> BeDefinitionId { get; set; }
        [RequiredArgument]
        public InArgument<object> ReaderState { get; set; }
        protected override void DoWork(SaveBPDefinitionStateInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ProcessManager processManager = new ProcessManager();
            if (inputArgument.ReaderState != null)
                processManager.SaveDefinitionObjectState(handle.SharedInstanceData.InstanceInfo.DefinitionID, inputArgument.BeDefinitionId.ToString(), inputArgument.ReaderState);
        }

        protected override SaveBPDefinitionStateInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new SaveBPDefinitionStateInput
            {
                BeDefinitionId = this.BeDefinitionId.Get(context),
                ReaderState = this.ReaderState.Get(context)
            };
        }
    }
}
