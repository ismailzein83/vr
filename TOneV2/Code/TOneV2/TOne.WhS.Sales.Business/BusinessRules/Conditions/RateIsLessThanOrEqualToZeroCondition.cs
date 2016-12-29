using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class RateIsLessThanOrEqualToZeroCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            if (zone.NormalRateToChange != null && zone.NormalRateToChange.NormalRate <= 0)
            {
                context.Message = String.Format("The rate of zone {0} must be greater than 0", zone.ZoneName);
                return false;
            }

            if (zone.OtherRatesToChange != null && zone.OtherRatesToChange.Any(x => x.NormalRate <= 0))
            {
                context.Message = String.Format("The other rate of zone {0} must be greater than 0", zone.ZoneName);
                return false;
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            DataByZone zone = (target as DataByZone);
            return String.Format("The rate of zone {0} must be greater than 0", zone.ZoneName);
        }
    }
}
