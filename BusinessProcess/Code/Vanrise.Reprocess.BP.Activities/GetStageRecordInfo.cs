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
    public sealed class GetStageRecordInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ReprocessStage> Stage { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<StageRecordInfo>> StageRecordInfos { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ReprocessStage stage = this.Stage.Get(context);

            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, string.Format("{0} Preparation is started.", stage.StageName), null);
            var executionContext = new ReprocessStageActivatorPreparingContext(stage.StageName, context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue ? context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value : context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID);

            List<StageRecordInfo> data = stage.Activator.GetStageRecordInfo(executionContext);
            this.StageRecordInfos.Set(context, data);

            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, string.Format("{0} Preparation is done.", this.Stage.Get(context).StageName), null);
        }

        private class ReprocessStageActivatorPreparingContext : IReprocessStageActivatorPreparingContext
        {
            string _currentStageName;
            long _processInstanceId;
            public ReprocessStageActivatorPreparingContext(string currentStageName, long processInstanceId)
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