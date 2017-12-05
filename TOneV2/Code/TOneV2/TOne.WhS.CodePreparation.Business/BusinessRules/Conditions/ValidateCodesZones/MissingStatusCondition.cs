using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class MissingStatusCondition : BusinessRuleCondition
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
                if (!string.IsNullOrEmpty(importedCode.Code) && importedCode.Status == null)
                    invalidCodes.Add(importedCode.Code);
            }
            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Status is missing for the following code(s): {0}.", string.Join(", ", invalidCodes));
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