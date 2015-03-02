using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class ActionExecutionPath<T>
    {
        public ActionExecutionStep<T> FirstStep { get; set; }

        public ActionExecutionStep<T> GetStep(Type actionType)
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

    public class ActionExecutionStepsByActionType<T>
    {
        public ActionExecutionStepsByActionType()
        {
            Steps = new Dictionary<Type, ActionExecutionStep<T>>();
        }
        public Dictionary<Type, ActionExecutionStep<T>> Steps { get; private set; }
    }
}
