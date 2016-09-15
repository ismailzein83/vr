using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class ServiceEEDGreaterThanZoneEED : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            DataByZone zone = target as DataByZone;

            string serviceEEDString = "NULL";

            if (zone.SaleZoneServiceToAdd != null)
            {
                if (zone.SaleZoneServiceToAdd.EED.HasValue)
                    serviceEEDString = zone.SaleZoneServiceToAdd.EED.Value.Date.ToShortDateString();
            }
            else
                serviceEEDString = zone.SaleZoneServiceToClose.CloseEffectiveDate.Date.ToShortDateString();

            return String.Format("EED '{0}' of the service of zone '{1}' must be less than or equal to EED '{2}' of the zone", serviceEEDString, zone.ZoneName, zone.EED.Value.ToShortDateString());
        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            if (zone.EED.HasValue)
            {
                if (zone.SaleZoneServiceToAdd != null)
                {
                    if (!zone.SaleZoneServiceToAdd.EED.HasValue || zone.SaleZoneServiceToAdd.EED.Value > zone.EED.Value)
                        return false;
                }
                else if (zone.SaleZoneServiceToClose != null)
                {
                    if (zone.SaleZoneServiceToClose.CloseEffectiveDate > zone.EED.Value)
                        return false;
                }
            }
            
            return true;
        }
    }
}
