using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class FinalizeStage : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IReprocessStageActivator> StageActivator { get; set; }

        [RequiredArgument]
        public InArgument<string> StageName { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> BatchStart { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            long currentProcessId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue ? context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value : context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            var executionContext = new ReprocessStageActivatorFinalizingContext(this.StageName.Get(context), currentProcessId, this.BatchStart.Get(context));

            this.StageActivator.Get(context).FinalizeStage(executionContext);
        }

        private class ReprocessStageActivatorFinalizingContext : IReprocessStageActivatorFinalizingContext
        {
            string _currentStageName;
            long _processInstanceId;
            DateTime _batchStart;

            public ReprocessStageActivatorFinalizingContext(string currentStageName, long processInstanceId, DateTime batchStart)
            {
                _currentStageName = currentStageName;
                _processInstanceId = processInstanceId;
                _batchStart = batchStart;
            }

            public long ProcessInstanceId
            {
                get { return _processInstanceId; }
            }

            public string CurrentStageName
            {
                get { return _currentStageName; }
            }

            public DateTime BatchStart
            {
                get { return _batchStart; }
            }
        }
    }
}