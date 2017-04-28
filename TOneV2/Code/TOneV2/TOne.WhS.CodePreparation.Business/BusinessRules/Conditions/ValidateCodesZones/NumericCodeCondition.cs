using System;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class NumericCodeCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target is ImportedCode);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode importedCode = context.Target as ImportedCode;

            if (!Vanrise.Common.Utilities.IsNumeric(importedCode.Code, 0))
            {
                context.Message = string.Format("Code {0} is not numeric", importedCode.Code);
                return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
