using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class ExcludeItemAction : BusinessRuleAction
    {
        public override void Execute(IBusinessRuleActionExecutionContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("Target");

            context.Target.SetExcluded();
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Warning;
        }
    }
}
