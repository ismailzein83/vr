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
            ZoneToProcess zoneTopProcess = context.Target as ZoneToProcess;
            DateTime minExistingZonesBED = DateTime.MinValue;
            if (zoneTopProcess.ExistingZones.Count() > 0)
                minExistingZonesBED = zoneTopProcess.ExistingZones.Min(item => item.BED);

            if (zoneTopProcess.CodesToAdd != null)
            {

                foreach (CodeToAdd codeToAdd in zoneTopProcess.CodesToAdd)
                {
                    if (codeToAdd.ChangedExistingCodes.Count() > 0)
                    {
                        ExistingCode changedExistingCode = codeToAdd.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToAdd.Code);
                        
                        if (minExistingZonesBED != DateTime.MinValue && minExistingZonesBED.Date > DateTime.Today.Date && changedExistingCode != null &&
                             !codeToAdd.ZoneName.Equals(changedExistingCode.ParentZone.Name, StringComparison.InvariantCultureIgnoreCase))
                            return false;
                    }
                }
            }

            if (zoneTopProcess.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in zoneTopProcess.CodesToMove)
                {
                    if (minExistingZonesBED != DateTime.MinValue && minExistingZonesBED > DateTime.Today.Date)
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
