using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class AddMoveCodeToPendingClosedZoneCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ExistingZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ExistingZone existingZone = context.Target as ExistingZone;
            if (existingZone.AddedCodes.Count() > 0)
                return !existingZone.EED.HasValue;
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Can not move or add code to the pending closed zone {0}", (target as ExistingZone).ZoneEntity.Name);
        }

    }
}
