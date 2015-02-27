using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RuleActionExecutionPath
    {
        public RuleActionExecutionStep FirstStep { get; set; }

        public RuleActionExecutionStepsByActionType GetAllSteps()
        {
            RuleActionExecutionStepsByActionType allSteps = new RuleActionExecutionStepsByActionType();
            var current = this.FirstStep;
            while (current != null)
            {
                var actionType = current.Action.GetType();
                if (!allSteps.Steps.ContainsKey(actionType))
                    allSteps.Steps.Add(actionType, current);
                current = current.NextStep;
            }
            return allSteps;
        }
    }

    public class RuleActionExecutionStepsByActionType
    {
        public RuleActionExecutionStepsByActionType()
        {
            Steps = new Dictionary<Type, RuleActionExecutionStep>();
        }
        public Dictionary<Type, RuleActionExecutionStep> Steps { get; private set; }
    }
}