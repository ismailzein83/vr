using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NormalRateBEDLessThanValidBEDCondition : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            DataByZone zone = (target as DataByZone);
            return String.Format("BED ({0}) of the normal rate of zone {1} must be greater than or equal to BED ({2}) of the zone", zone.NormalRateToChange.BED, zone.ZoneName, zone.BED);
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
                if (zone.NormalRateToChange.BED == default(DateTime))
                    return false;

                var beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn };

                if (zone.NormalRateToChange.BED < UtilitiesManager.GetMaxDate(beginEffectiveDates).Value)
                    return false;
            }

            return true;
        }
    }
}
