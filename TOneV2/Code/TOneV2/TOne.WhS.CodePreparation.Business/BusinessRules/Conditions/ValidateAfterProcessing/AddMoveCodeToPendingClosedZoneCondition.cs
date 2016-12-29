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
    public class AddMoveCodeToPendingClosedZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            if (zoneToProcess.CodesToAdd.Count() > 0 && zoneToProcess.EED.HasValue)
            {
                context.Message = string.Format("Can not add code to pending closed zone '{0}'", zoneToProcess.ZoneName);
                return false;
            }


            if (zoneToProcess.CodesToMove.Count() > 0 && zoneToProcess.EED.HasValue)
            {
                context.Message = string.Format("Can not move code to pending closed zone '{0}'", zoneToProcess.ZoneName);
                return false;
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Can not move or add code to the pending closed zone {0}", (target as ZoneToProcess).ZoneName);
        }

    }
}
