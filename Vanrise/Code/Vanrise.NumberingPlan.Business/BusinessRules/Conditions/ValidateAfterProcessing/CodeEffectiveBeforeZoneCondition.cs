using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class CodeEffectiveBeforeZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zone = context.Target as ZoneToProcess;
            var invalidCodes = new HashSet<string>();

            foreach (CodeToAdd codeToAdd in zone.CodesToAdd)
            {
                if (zone.ExistingZones.Any(item => item.BED > codeToAdd.BED))
                    invalidCodes.Add(codeToAdd.Code);
            }

            foreach (CodeToMove codeToMove in zone.CodesToMove)
            {
                if (zone.ExistingZones.Any(item => item.BED > codeToMove.BED))
                    invalidCodes.Add(codeToMove.Code);
            }

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Can not add or move code(s) ({0}) in zone '{1}' with effective dates less than the effective date of its zone.", string.Join(",", invalidCodes), zone.ZoneName);
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
