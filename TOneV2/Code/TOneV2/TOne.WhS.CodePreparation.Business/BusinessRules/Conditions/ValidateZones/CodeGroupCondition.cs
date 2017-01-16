using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodeGroupCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CodeToAdd != null || target as CodeToClose != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var result = true;

            IRuleTarget target = context.Target;

            CodeToAdd codeToAdd = target as CodeToAdd;
            CodeToClose codeToClose = target as CodeToClose;

            if (codeToAdd != null && codeToAdd.CodeGroup == null)
            {
                result = false;
                context.Message = string.Format("Can not add Code {0} because it is not assigned to a code group", codeToAdd.Code);
            }
            else if (codeToClose != null && codeToClose.CodeGroup == null)
            {
                result = false;
                context.Message = string.Format("Can not close Code {0} because it is not assigned to a code group", codeToClose.Code);
            }

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} not assigned to a code group", (target as CodeToAdd).Code);
        }
    }
}
