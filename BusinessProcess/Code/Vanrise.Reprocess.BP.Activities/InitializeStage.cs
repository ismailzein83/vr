using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class InitializeStage : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<ReprocessStage> Stage { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, object>> InitializationOutputByStage { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            ReprocessStage stage = this.Stage.Get(context.ActivityContext);
            Dictionary<string, object> initializationOutputByStage = this.InitializationOutputByStage.Get(context.ActivityContext);
            long currentProcessId = context.ActivityContext.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            var initializatingContext = new ReprocessStageActivatorInitializingContext(currentProcessId);
            var initStageOutput = stage.Activator.InitializeStage(initializatingContext);

            if (initializationOutputByStage.ContainsKey(stage.StageName))
                throw new VRBusinessException(string.Format("initializationOutputByStage already contains stage: '{0}'", stage.StageName));

            initializationOutputByStage.Add(stage.StageName, initStageOutput);
        }

        private class ReprocessStageActivatorInitializingContext : IReprocessStageActivatorInitializingContext
        {
            long _processId;

            public ReprocessStageActivatorInitializingContext(long processId)
            {
                _processId = processId;
            }

            public long ProcessId { get { return _processId; } }
        }
    }
}
