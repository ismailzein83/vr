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
    public class AddMoveCodeToPendingClosedZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            var invalidCodes = new List<string>();
            if (zoneToProcess.EED.HasValue)
            {
                if (zoneToProcess.CodesToAdd.Count() > 0)
                    invalidCodes.AddRange(zoneToProcess.CodesToAdd.Select(item => item.Code));
                if (zoneToProcess.CodesToMove.Count() > 0)
                    invalidCodes.AddRange(zoneToProcess.CodesToMove.Select(item => item.Code));

                if (invalidCodes.Count > 0)
                {
                    context.Message = string.Format("Can not add or move code(s) ({0}) to zone '{1}' which is pending closed in {2}.", string.Join(", ", invalidCodes), zoneToProcess.ZoneName, zoneToProcess.EED.Value);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
