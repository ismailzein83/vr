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
            Dictionary<string, ExistingZone> existingZonesByZoneName = cpContext.ExistingZonesByZoneName;  //TODO: Zone.BED must be filled in ZoneToProcess

            ZoneToProcess zoneTopProcess = context.Target as ZoneToProcess;

            if (zoneTopProcess.CodesToAdd != null)
            {
                foreach (CodeToAdd codeToAdd in zoneTopProcess.CodesToAdd)
                {
                    if (codeToAdd.ChangedExistingCodes.Count() > 0)
                    {
                        ExistingZone existingZoneInContext;
                        existingZonesByZoneName.TryGetValue(codeToAdd.ZoneName, out existingZoneInContext);

                        ExistingCode changedExistingCode= codeToAdd.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToAdd.Code);

                        if (existingZoneInContext != null && existingZoneInContext.BED > DateTime.Today && changedExistingCode != null &&
                            !codeToAdd.ZoneName.Equals(changedExistingCode.ParentZone.Name,StringComparison.InvariantCultureIgnoreCase))
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
