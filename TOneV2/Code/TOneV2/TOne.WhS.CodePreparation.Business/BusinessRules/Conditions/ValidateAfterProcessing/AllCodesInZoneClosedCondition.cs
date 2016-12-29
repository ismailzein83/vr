using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class AllCodesInZoneClosedCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null || target as NotImportedZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            bool result;
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            if (zoneToProcess != null)
            {
                result = !(zoneToProcess.ChangeType == Entities.ZoneChangeType.Deleted);
               
                if (result == false)
                    context.Message = string.Format("All codes in zone '{0}' are closed. Zone '{0}' will be closed", zoneToProcess.ZoneName);

                return result;
            }

            NotImportedZone notImportedZone = context.Target as NotImportedZone;
            result = !notImportedZone.HasChanged;
            if (result == false)
                context.Message = string.Format("All codes in zone '{0}' are closed. Zone '{0}' will be closed", notImportedZone.ZoneName);

            return result;

        }

        public override string GetMessage(IRuleTarget target)
        {
            string zoneName = target as ZoneToProcess != null ? (target as ZoneToProcess).ZoneName : (target as NotImportedZone).ZoneName;

            return string.Format("All codes in zone {0} are closed, zone {0} will be closed", zoneName);
        }

    }
}
