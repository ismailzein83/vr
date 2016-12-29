using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class ServiceBEDLessThanValidBEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            if (zone.SaleZoneServiceToAdd != null)
            {
                if (zone.SaleZoneServiceToAdd.BED == default(DateTime))
                {
                    context.Message = string.Format("BED of the service of zone '{0}' is missing", zone.ZoneName);
                    return false;
                }

                var beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn };

                if (zone.SaleZoneServiceToAdd.BED < UtilitiesManager.GetMaxDate(beginEffectiveDates).Value)
                {
                    context.Message = string.Format("BED '{0}' of the service of zone '{1}' must be greater than or equal to BED '{2}' of the zone", zone.SaleZoneServiceToAdd.BED.Date.ToShortDateString(), zone.ZoneName, zone.BED.Date.ToShortDateString());
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            var zone = target as DataByZone;

            if (zone.SaleZoneServiceToAdd.BED == default(DateTime))
                return String.Format("BED of the service of zone '{0}' is missing", zone.ZoneName);

            return String.Format("BED '{0}' of the service of zone '{1}' must be greater than or equal to BED '{2}' of the zone", zone.SaleZoneServiceToAdd.BED.Date.ToShortDateString(), zone.ZoneName, zone.BED.Date.ToShortDateString());
        }
    }
}
