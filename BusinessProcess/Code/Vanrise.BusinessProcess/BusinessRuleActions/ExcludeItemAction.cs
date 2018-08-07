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
        public override Guid BPBusinessRuleActionTypeId
        {
            get { return new Guid("BA3427FE-B8BE-4546-B433-CE0D8CE9FCB1"); }
        }
        public override void Execute(IBusinessRuleActionExecutionContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("Target");

            var target = context.Target as IExclude;
            if (target == null)
                throw new InvalidOperationException("Trying to execute exclude action on a target that doesn't implement IExclude");
            target.SetAsExcluded();
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Warning;
        }
    }
}
