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
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            var invalidCodes = new List<string>();
            if (zoneToProcess.ChangeType == ZoneChangeType.New || zoneToProcess.ChangeType == ZoneChangeType.Renamed || zoneToProcess.BED <= cpContext.EffectiveDate)
                return true;

            if (zoneToProcess.CodesToAdd.Count() > 0 )
                invalidCodes.AddRange(zoneToProcess.CodesToAdd.Select(item => item.Code));

            if (zoneToProcess.CodesToMove.Count() > 0 )
                invalidCodes.AddRange(zoneToProcess.CodesToMove.Select(item => item.Code));

            if(invalidCodes.Count>0)
            {
                context.Message = string.Format("Can not add or move codes ({0}) to zone '{1}' which is pending effective in {3}. Adding or moving code to this zone must be on {3} or after.");
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
