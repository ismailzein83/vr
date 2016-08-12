using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;
using Vanrise.Reprocess.Business;
using Vanrise.Queueing;
using System.Linq;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class GetStageActivator : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> StageName { get; set; }

        [RequiredArgument]
        public InArgument<ReprocessDefinition> ReprocessDefinition { get; set; }

        [RequiredArgument]
        public OutArgument<IReprocessStageActivator> StageActivator { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var reprocessDefinition = this.ReprocessDefinition.Get(context);
            var stageName = this.StageName.Get(context);

            if (reprocessDefinition == null)
                throw new ArgumentNullException("reprocessDefinition");

            if (reprocessDefinition.Settings == null)
                throw new ArgumentNullException("reprocessDefinition.Settings");

            if (reprocessDefinition.Settings.StageNames == null)
                throw new NullReferenceException(String.Format("reprocessDefinition.Settings.StageNames. ReprocessDefinitionId '{0}'", reprocessDefinition.ReprocessDefinitionId));

            var stages = new QueueExecutionFlowDefinitionManager().GetFlowStages(reprocessDefinition.Settings.ExecutionFlowDefinitionId);
            if (stages == null)
                throw new NullReferenceException(String.Format("stages '{0}'", reprocessDefinition.Settings.ExecutionFlowDefinitionId));

            var currentStage = stages.Values.FirstOrDefault(itm => string.Compare(itm.StageName, stageName, true) == 0);
            if (currentStage == null)
                throw new NullReferenceException(String.Format("currentStage. StageName '{0}'", stageName));

            var reprocessActivator = currentStage.QueueActivator as IReprocessStageActivator;

            this.StageActivator.Set(context, reprocessActivator);
        }
    }
}