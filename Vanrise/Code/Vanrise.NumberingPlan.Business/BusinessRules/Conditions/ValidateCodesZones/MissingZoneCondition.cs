using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class MissingZoneCondition : BusinessRuleCondition
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
                if (string.IsNullOrEmpty(importedCode.ZoneName) && !string.IsNullOrEmpty(importedCode.Code))
                    invalidCodes.Add(importedCode.Code);

            }
            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Zone is missing for the following code(s): {0}.", string.Join(", ", invalidCodes));
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
