using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Common;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class CommitChanges : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<IReprocessStageActivator> StageActivator { get; set; }

        [RequiredArgument]
        public InArgument<string> StageName { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, object>> InitializationOutputByStage { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            string stageName = this.StageName.Get(context.ActivityContext);
            DateTime from = this.From.Get(context.ActivityContext);
            DateTime to = this.To.Get(context.ActivityContext);
            IReprocessStageActivator stageActivator = this.StageActivator.Get(context.ActivityContext);
            Dictionary<string, object> initializationOutputByStage = this.InitializationOutputByStage.Get(context.ActivityContext);

            context.ActivityContext.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Starting Commit Changes for stage {0} Batch Start: {1}, Batch End : {2}", stageName, from.ToString("yyyy-MM-dd HH:mm:ss"), to.ToString("yyyy-MM-dd HH:mm:ss"));
            object initializationStageOutput = initializationOutputByStage.GetRecord(stageName);

            var initializatingContext = new ReprocessStageActivatorCommitChangesContext(initializationStageOutput, from, to);
            stageActivator.CommitChanges(initializatingContext);
            context.ActivityContext.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finishing Commit Changes for stage {0} Batch Start: {1}, Batch End : {2}", stageName, from.ToString("yyyy-MM-dd HH:mm:ss"), to.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private class ReprocessStageActivatorCommitChangesContext : IReprocessStageActivatorCommitChangesContext
        {
            object _initializationStageOutput;
            DateTime _from, _to;

            public ReprocessStageActivatorCommitChangesContext(object initializationStageOutput, DateTime from, DateTime to)
            {
                _initializationStageOutput = initializationStageOutput;
                _from = from;
                _to = to;
            }

            public object InitializationStageOutput { get { return _initializationStageOutput; } }

            public DateTime From { get { return _from; } }

            public DateTime To { get { return _to; } }
        }
    }
}