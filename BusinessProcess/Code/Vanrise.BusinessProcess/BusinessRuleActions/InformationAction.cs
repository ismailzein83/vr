using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class InformationAction : BusinessRuleAction
    {
        public override Guid BPBusinessRuleActionTypeId
        {
            get { return new Guid("72C926F1-D019-408F-84AF-6613D2033473"); }
        }
        public override void Execute(IBusinessRuleActionExecutionContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("context.Target");
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Information;
        }
        public override string GetDescription()
        {
            return "Information";
        }
    }
}
