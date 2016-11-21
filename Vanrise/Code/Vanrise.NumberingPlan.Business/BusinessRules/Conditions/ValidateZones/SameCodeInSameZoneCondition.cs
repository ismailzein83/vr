using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class SameCodeInSameZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            if (zoneToProcess.CodesToAdd.GroupBy(x => x.Code).Any(x => x.Count() > 1))
                return false;

            if (zoneToProcess.CodesToClose.GroupBy(x => x.Code).Any(x => x.Count() > 1))
                return false;

            if (zoneToProcess.CodesToMove.Any(x => x.ZoneName.Equals(x.OldZoneName, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has duplicate code", (target as ZoneToProcess).ZoneName);
        }
    }
}
