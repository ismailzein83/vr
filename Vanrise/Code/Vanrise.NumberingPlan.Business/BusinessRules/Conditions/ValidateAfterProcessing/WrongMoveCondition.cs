using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;


namespace Vanrise.NumberingPlan.Business
{
    public class WrongMoveCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CodeToAdd != null && target as CodeToMove == null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CodeToAdd codeToAdd = context.Target as CodeToAdd;

            return !(codeToAdd.ChangedExistingCodes != null && codeToAdd.ChangedExistingCodes.Count() > 0);
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} already exists", (target as CodeToAdd).Code);
        }
 
    }
}
