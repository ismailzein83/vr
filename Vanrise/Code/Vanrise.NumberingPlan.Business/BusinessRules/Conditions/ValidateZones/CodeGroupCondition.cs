using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class CodeGroupCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CodeToAdd != null || target as CodeToClose != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var returnedResult = false;

            IRuleTarget target = context.Target;
           
            CodeToAdd codeToAdd = target as CodeToAdd;
            CodeToClose codeToClose = target as CodeToClose;

            if (codeToAdd != null)
                returnedResult = codeToAdd.CodeGroup != null;

            else if (codeToClose != null)
                returnedResult = codeToClose.CodeGroup != null;

            return returnedResult;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} not assigned to a code group", (target as CodeToAdd).Code);
        }
    }
}
