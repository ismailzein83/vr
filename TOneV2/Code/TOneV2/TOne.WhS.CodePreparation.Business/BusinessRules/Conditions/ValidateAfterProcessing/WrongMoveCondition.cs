using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;


namespace TOne.WhS.CodePreparation.Business
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
            return string.Format("Can not move code {0} without deleting it from existing zone", (target as CodeToAdd).Code);
        }
 
    }
}
