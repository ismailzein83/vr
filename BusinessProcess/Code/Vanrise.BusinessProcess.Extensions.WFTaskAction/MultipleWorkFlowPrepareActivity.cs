using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;

namespace Vanrise.BusinessProcess.Extensions.WFTaskAction
{
    public sealed class MultipleWorkFlowPrepareActivity : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<Dictionary<string, object>> EvaluatedExpressions { get; set; }
        public OutArgument<Dictionary<string, object>> PreparedEvaluatedExpressions { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var evaluatedExpressions = context.GetValue(this.EvaluatedExpressions);
            if (evaluatedExpressions != null)
            {
                var clonedEvaluatedExpressions = Vanrise.Common.Utilities.CloneObject<Dictionary<string, object>>(evaluatedExpressions);
                //clonedEvaluatedExpressions.Add("TaskName", task.Name);
                //    inputArgument.InputArgument.MapExpressionValues(clonedEvaluatedExpressions);
                PreparedEvaluatedExpressions.Set(context, clonedEvaluatedExpressions);
            }
        }
    }
}
