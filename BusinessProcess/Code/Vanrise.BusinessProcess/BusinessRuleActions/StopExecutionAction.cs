using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class StopExecutionAction : BusinessRuleAction
    {

        public override void Execute(IBusinessRuleActionExecutionContext context)
        {
            context.StopExecution = true;
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Error;
        }
    }
}
