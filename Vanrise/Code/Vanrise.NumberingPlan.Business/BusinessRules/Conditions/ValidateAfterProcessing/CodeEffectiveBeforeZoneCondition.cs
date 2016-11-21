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

            if (zone.CodesToAdd != null)
            {
                foreach (CodeToAdd codeToAdd in zone.CodesToAdd)
                {
                    if (zone.ExistingZones.Any(item => item.BED > codeToAdd.BED))
                        return false;
                }

            }


            if (zone.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in zone.CodesToMove)
                {
                    if (zone.ExistingZones.Any(item => item.BED > codeToMove.BED))
                        return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a code with effective date less than the effective date of the zone", (target as ZoneToProcess).ZoneName);
        }

    }
}
