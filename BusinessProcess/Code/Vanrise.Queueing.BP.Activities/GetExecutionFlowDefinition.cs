using System;
using System.Activities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.BP.Activities
{
    public sealed class GetExecutionFlowDefinition : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> ExecutionFlowDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<QueueExecutionFlowDefinition> ExecutionFlowDefinition { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            QueueExecutionFlowDefinitionManager manager = new QueueExecutionFlowDefinitionManager();
            QueueExecutionFlowDefinition executionFlowDefinition = manager.GetExecutionFlowDefinition(this.ExecutionFlowDefinitionId.Get(context));

            this.ExecutionFlowDefinition.Set(context, executionFlowDefinition);
        }
    }
}