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
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            if (zoneToProcess.ChangeType == ZoneChangeType.New || zoneToProcess.ChangeType == ZoneChangeType.Renamed)
                return true;

            if (zoneToProcess.CodesToAdd.Count() > 0 && zoneToProcess.BED > DateTime.Today.Date)
            {
                context.Message = string.Format("Cannot add codes to the pending effective zone {0}", zoneToProcess.ZoneName);
                return false;
            }

            if (zoneToProcess.CodesToMove.Count() > 0 && zoneToProcess.BED > DateTime.Today.Date)
            {
                context.Message = string.Format("Cannot move codes to the pending effective zone {0}", zoneToProcess.ZoneName);
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
