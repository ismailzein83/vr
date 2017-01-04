using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
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

            foreach (CodeToAdd codeToAdd in zone.CodesToAdd)
            {
                if (zone.ExistingZones.Any(item => item.BED > codeToAdd.BED))
                {
                    context.Message = string.Format("Can not add code {0} at zone {1} with an effective date less than the effective date of its zone", codeToAdd.Code, zone.ZoneName);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zone.CodesToMove)
            {
                if (zone.ExistingZones.Any(item => item.BED > codeToMove.BED))
                {
                    context.Message = string.Format("Can not move code {0} at zone {1} with an effective date less than the effective date of its zone", codeToMove.Code, zone.ZoneName);
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
