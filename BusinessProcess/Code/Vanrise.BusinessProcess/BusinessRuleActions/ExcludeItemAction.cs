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

            //context.Target.SetExcluded(2);
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Warning;
        }
    }
}
