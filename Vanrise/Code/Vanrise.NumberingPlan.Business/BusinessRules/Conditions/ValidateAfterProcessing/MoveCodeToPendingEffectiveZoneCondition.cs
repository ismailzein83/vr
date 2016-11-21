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
    public class MoveCodeToPendingEffectiveZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneTopProcess = context.Target as ZoneToProcess;

            if (zoneTopProcess.ChangeType == ZoneChangeType.New || zoneTopProcess.ChangeType == ZoneChangeType.Renamed)
                return true;

            if (zoneTopProcess.CodesToAdd != null && zoneTopProcess.CodesToAdd.Count() > 0)
            {
                if (zoneTopProcess.BED > DateTime.Today.Date)
                    return false;
            }

            if (zoneTopProcess.CodesToMove != null && zoneTopProcess.CodesToMove.Count() > 0)
            {
                return !(zoneTopProcess.BED > DateTime.Today.Date);
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Can not move or add code to the pending effective zone {0}", (target as ZoneToProcess).ZoneName);
        }

    }
}
