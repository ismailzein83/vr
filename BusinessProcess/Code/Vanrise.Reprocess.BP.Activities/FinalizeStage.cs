using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public class FinalizeStageInput
    {
        public ReprocessStage Stage { get; set; }

        public long CurrentProcessId { get; set; }
    }

    public class FinalizeStageOutput
    {
    }

    public sealed class FinalizeStage : DependentAsyncActivity<FinalizeStageInput, FinalizeStageOutput>
    {
        [RequiredArgument]
        public InArgument<ReprocessStage> Stage { get; set; }

        protected override FinalizeStageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new FinalizeStageInput()
            {
                Stage = this.Stage.Get(context),
                CurrentProcessId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue ? context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value : context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID
            };
        }

        protected override FinalizeStageOutput DoWorkWithResult(FinalizeStageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var executionContext = new ReprocessStageActivatorFinalizingContext(inputArgument.Stage.StageName, inputArgument.CurrentProcessId);

            inputArgument.Stage.Activator.FinalizeStage(executionContext);
            return new FinalizeStageOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinalizeStageOutput result)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, string.Format("{0} finalization is done.", this.Stage.Get(context).StageName), null);
        }

        private class ReprocessStageActivatorFinalizingContext : IReprocessStageActivatorFinalizingContext
        {
            string _currentStageName;
            long _processInstanceId;

            public ReprocessStageActivatorFinalizingContext(string currentStageName, long processInstanceId)
            {
                _currentStageName = currentStageName;
                _processInstanceId = processInstanceId;
            }

            public long ProcessInstanceId
            {
                get { return _processInstanceId; }
            }

            public string CurrentStageName
            {
                get { return _currentStageName; }
            }
        }


    }
}