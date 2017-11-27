using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class InvalidStatusCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllImportedCodes != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllImportedCodes allImportedCodes = context.Target as AllImportedCodes;
            var invalidCodes = new HashSet<string>();
            foreach (var importedCode in allImportedCodes.ImportedCodes)
            {
                if (!string.IsNullOrEmpty(importedCode.Code) && importedCode.Status == ImportType.Invalid)
                    invalidCodes.Add(importedCode.Code);
            }
            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Status is invalid for the following code(s): {0}.", string.Join(", ", invalidCodes));
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
