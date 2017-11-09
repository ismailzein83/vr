using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class MissingCodeCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllImportedCodes != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllImportedCodes allImportedCodes = context.Target as AllImportedCodes;
            var invalidZones = new HashSet<string>();
            foreach (var importedCode in allImportedCodes.ImportedCodes)
            {
                if (string.IsNullOrEmpty(importedCode.Code))
                    invalidZones.Add(importedCode.ZoneName);
            }
            if (invalidZones.Count > 0)
            {
                context.Message = string.Format("Codes are missing for the following zone(s): {0}.", string.Join(", ", invalidZones));
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