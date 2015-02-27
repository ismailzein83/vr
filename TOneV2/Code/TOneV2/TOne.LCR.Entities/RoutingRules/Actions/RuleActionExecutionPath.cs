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

        public RuleActionExecutionStep GetStep(Type actionType)
        {
            var current = this.FirstStep;
            while (current != null)
            {
                if (current.Action.GetType().Equals(actionType))
                    return current;
                else
                    current = current.NextStep;
            }
            return null;
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