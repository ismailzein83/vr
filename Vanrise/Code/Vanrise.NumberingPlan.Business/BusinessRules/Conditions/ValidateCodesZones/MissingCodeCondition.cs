using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
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
            var invalidRecords = 0;
            var invalidZones = new HashSet<string>();
            foreach (var importedCode in allImportedCodes.ImportedCodes)
            {
                if (string.IsNullOrEmpty(importedCode.Code))
                {
                    if (string.IsNullOrEmpty(importedCode.ZoneName))
                        invalidRecords++;
                    else invalidZones.Add(importedCode.ZoneName);
                }
            }
            if (invalidZones.Count > 0)
            {
                context.Message = string.Format("Codes are missing for the following zone(s): {0}.", string.Join(", ", invalidZones));
                return false;
            }
            if (invalidRecords > 0)
            {
                context.Message = string.Format("Codes and zone are missing for {0} record(s).", invalidRecords);
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
