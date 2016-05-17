using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.CodePreparation.Entities;

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
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();
            Dictionary<string, ExistingZone> existingZonesByZoneName = cpContext.ExistingZonesByZoneName; //TODO: Zone.BED must be filled in ZoneToProcess
            ZoneToProcess zone = context.Target as ZoneToProcess;

            if (zone.CodesToAdd != null)
            {
               

                foreach (CodeToAdd codeToAdd in zone.CodesToAdd)
                {
                    ExistingZone existingZoneInContext;
                    existingZonesByZoneName.TryGetValue(codeToAdd.ZoneName, out existingZoneInContext);
                    if (existingZoneInContext != null && existingZoneInContext.BED > codeToAdd.BED)
                        return false;
                }

            }


            if (zone.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in zone.CodesToMove)
                {
                    ExistingZone existingZoneInContext;
                    existingZonesByZoneName.TryGetValue(codeToMove.ZoneName, out existingZoneInContext);

                    if (existingZoneInContext != null && existingZoneInContext.BED > codeToMove.BED)
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
