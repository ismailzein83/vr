using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NormalRateEEDGreaterThanZoneEED : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            return String.Format("NormalRate.EED > Zone.EED");
        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            if (zone.NormalRateToChange != null)
            {
                if (zone.NormalRateToChange.EED > zone.EED)
                    return false;
            }
            else if (zone.NormalRateToClose != null)
            {
                if (zone.NormalRateToClose.CloseEffectiveDate > zone.EED)
                    return false;
            }

            return true;
        }
    }
}
