using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.CodePreparation.Business
{
    public class AllCodesInZoneClosedCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ExistingZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ExistingZone existingZone = context.Target as ExistingZone;
            if (existingZone.ChangedZone != null)
                return false;

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("All codes in zone {0} has been closed, zone {0} will be closed", (target as ExistingZone).ZoneEntity.Name);
        }

    }
}
