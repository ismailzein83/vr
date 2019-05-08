using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction
{

    public sealed class MultipleWorkFlowPrepareInputArgumentActivity : CodeActivity
    {
        public InArgument<Dictionary<string, object>> PreparedEvaluatedExpressions { get; set; }
        public InOutArgument<MultipleWorkFlowInput> InputArgument { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var inputArgument = context.GetValue(this.InputArgument);
            var preparedEvaluatedExpressions = context.GetValue(this.PreparedEvaluatedExpressions);
            if(preparedEvaluatedExpressions != null)
              inputArgument.InputArgument.MapExpressionValues(preparedEvaluatedExpressions);
            inputArgument.InputArgument.UserId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
        }
    }
}
