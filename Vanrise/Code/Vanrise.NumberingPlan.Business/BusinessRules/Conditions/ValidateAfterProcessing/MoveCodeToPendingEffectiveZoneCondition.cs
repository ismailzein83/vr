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
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            var invalidCodes = new List<string>();
            if (zoneToProcess.ChangeType == ZoneChangeType.New || zoneToProcess.ChangeType == ZoneChangeType.Renamed || zoneToProcess.BED <= cpContext.EffectiveDate)
                return true;

            if (zoneToProcess.CodesToAdd.Count() > 0)
                invalidCodes.AddRange(zoneToProcess.CodesToAdd.Select(item => item.Code));

            if (zoneToProcess.CodesToMove.Count() > 0)
                invalidCodes.AddRange(zoneToProcess.CodesToMove.Select(item => item.Code));

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Can not add or move code(s) ({0}) to zone '{1}' which is pending effective in {2}. Adding or moving code to this zone must be on {2} or after.", string.Join(", ", invalidCodes), zoneToProcess.ZoneName, zoneToProcess.BED);
                return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
