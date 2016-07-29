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
    public class RateEEDGreaterThanZoneEEDCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            DataByZone zone = context.Target as DataByZone;

            if (zone.EED != null)
            {
                foreach (RateToChange rateToChange in zone.RatesToChange)
                {
                    if (rateToChange.EED.VRGreaterThan(zone.EED))
                        return false;
                }

                foreach (RateToClose rateToClose in zone.RatesToClose)
                {
                    if (rateToClose.CloseEffectiveDate > zone.EED.Value)
                        return false;
                }
            }
            
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Trying to update a rate with EED greater than EED of zone {0}", (target as DataByZone).ZoneName);
        }
    }
}
