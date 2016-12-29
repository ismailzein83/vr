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

            var result = !(codeToAdd.ChangedExistingCodes != null && codeToAdd.ChangedExistingCodes.Count() > 0);

            if(result ==  false)
                context.Message = string.Format("Code {0} already exists", codeToAdd.Code);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} already exists", (target as CodeToAdd).Code);
        }
 
    }
}
