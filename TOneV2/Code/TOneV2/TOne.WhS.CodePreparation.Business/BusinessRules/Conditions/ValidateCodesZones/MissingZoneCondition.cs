using System;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class MissingZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode importedCode = context.Target as ImportedCode;
            var result = !string.IsNullOrEmpty(importedCode.ZoneName);
            if (result == false)
            {
                context.Message = !string.IsNullOrEmpty(importedCode.Code)
                    ? string.Format("Code {0} has a missing zone name", importedCode.Code)
                    : "One of the records is missing zone name and code";
            }
            return result;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
