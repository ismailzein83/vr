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
            return String.Format("Cannot add a normal rate with EED > EED of zone {0}", (target as DataByZone).ZoneName);
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
