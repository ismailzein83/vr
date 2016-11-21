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
    public class AllCodesInZoneClosedCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null || target as NotImportedZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            if (zoneToProcess != null)
                return !(zoneToProcess.ChangeType == Entities.ZoneChangeType.Deleted);

            NotImportedZone notImportedZone = context.Target as NotImportedZone;
            return !notImportedZone.HasChanged;

        }

        public override string GetMessage(IRuleTarget target)
        {
            string zoneName = target as ZoneToProcess != null ? (target as ZoneToProcess).ZoneName : (target as NotImportedZone).ZoneName;

            return string.Format("All codes in zone {0} are closed, zone {0} will be closed", zoneName);
        }

    }
}
