using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class WarningItemAction : BusinessRuleAction
    {
        public override Guid BPBusinessRuleActionTypeId
        {
            get { return new Guid("FBFE2B36-12F6-40C1-8163-26CFE2D23501"); }
        }
        public override void Execute(IBusinessRuleActionExecutionContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("context.Target");
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Warning;
        }
        public override string GetDescription()
        {
            return "Warning";
        }
    }
}
