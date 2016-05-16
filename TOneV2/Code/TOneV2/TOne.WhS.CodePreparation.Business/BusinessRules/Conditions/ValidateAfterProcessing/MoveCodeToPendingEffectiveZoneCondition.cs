using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class MoveCodeToPendingEffectiveZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();
            Dictionary<string, ExistingZone> existingZonesByZoneName = cpContext.ExistingZonesByZoneName;

            ZoneToProcess zoneTopProcess = context.Target as ZoneToProcess;

            if (zoneTopProcess.CodesToAdd != null)
            {
                foreach (CodeToAdd codeToAdd in zoneTopProcess.CodesToAdd)
                {
                    if (codeToAdd.ChangedExistingCodes != null)
                    {
                        ExistingZone existingZoneInContext;
                        existingZonesByZoneName.TryGetValue(codeToAdd.ZoneName, out existingZoneInContext);

                        if (existingZoneInContext != null && existingZoneInContext.BED > DateTime.Today &&
                            !codeToAdd.ZoneName.Equals(codeToAdd.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToAdd.Code).ParentZone.Name, StringComparison.InvariantCultureIgnoreCase))
                            return false;

                    }
                }
            }


            if (zoneTopProcess.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in zoneTopProcess.CodesToMove)
                {
                    ExistingZone existingZoneInContext;
                    existingZonesByZoneName.TryGetValue(codeToMove.ZoneName, out existingZoneInContext);
                    if (existingZoneInContext != null && existingZoneInContext.BED > DateTime.Today)
                        return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Can not move code to the pending effective zone {0}", (target as ZoneToProcess).ZoneName);
        }

    }
}
