using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class MoveClosedCodeToNewZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var invalidCodes = new List<string>();
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
            {
                if (codeToAdd.ChangedExistingCodes != null && codeToAdd.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToAdd.Code && item.CodeEntity.EED.HasValue))
                    invalidCodes.Add(codeToAdd.Code);
            }
            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Cannot move codes ({0}) in zone '{1}' because code(s) are pending closed.", string.Join(",", invalidCodes), zoneToProcess.ZoneName);
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
