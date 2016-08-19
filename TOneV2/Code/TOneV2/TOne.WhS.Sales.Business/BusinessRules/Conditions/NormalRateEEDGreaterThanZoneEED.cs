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
    public class NormalRateEEDGreaterThanZoneEED : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            DataByZone zone = (target as DataByZone);
            
            string normalRateEEDString = "NULL";
            if (zone.NormalRateToChange != null)
                if (zone.NormalRateToChange.EED.HasValue)
                    normalRateEEDString = zone.NormalRateToChange.EED.Value.ToShortDateString();
            else if (zone.NormalRateToClose != null)
                    normalRateEEDString = zone.NormalRateToClose.CloseEffectiveDate.ToShortDateString();

            return String.Format("EED ({0}) of the normal rate of zone {1} must be less than or equal to EED ({2}) of the zone", normalRateEEDString, zone.ZoneName, zone.EED);
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
                if (zone.EED.HasValue && zone.NormalRateToChange.EED > zone.EED.Value)
                    return false;
            }
            else if (zone.NormalRateToClose != null)
            {
                if (zone.EED.HasValue && zone.NormalRateToClose.CloseEffectiveDate > zone.EED.Value)
                    return false;
            }

            return true;
        }
    }
}
