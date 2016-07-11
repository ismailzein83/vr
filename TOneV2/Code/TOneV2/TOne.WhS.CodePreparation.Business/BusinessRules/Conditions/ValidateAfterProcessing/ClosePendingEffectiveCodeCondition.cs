using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class ClosePendingEffectiveCodeCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ExistingZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ExistingZone existingZone = context.Target as ExistingZone;
            foreach (ExistingCode existingCode in existingZone.ExistingCodes)
            {
                if (existingCode.ChangedCode != null && existingCode.BED > DateTime.Today.Date)
                    return false;
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a pending effective code that can not be closed", (target as ExistingZone).ZoneEntity.Name);
        }

    }
}
