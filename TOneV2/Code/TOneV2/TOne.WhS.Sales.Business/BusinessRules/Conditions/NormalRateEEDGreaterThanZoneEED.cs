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
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as DataByZone != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zoneData = context.Target as DataByZone;

            if (zoneData.NormalRateToChange != null)
            {
                if (zoneData.EED.HasValue && zoneData.NormalRateToChange.EED > zoneData.EED.Value)
                {
                    string zoneEED = UtilitiesManager.GetDateTimeAsString(zoneData.EED.Value);
                    string normalRateEED = UtilitiesManager.GetDateTimeAsString(zoneData.NormalRateToChange.EED.Value);
                    context.Message = string.Format("EED '{0}' of the normal rate of zone '{1}' must be less than or equal to EED '{2}' of the zone", normalRateEED, zoneData.ZoneName, zoneEED);
                    return false;
                }
            }
            else if (zoneData.NormalRateToClose != null)
            {
                if (zoneData.EED.HasValue && zoneData.NormalRateToClose.CloseEffectiveDate > zoneData.EED.Value)
                {
                    string zoneEED = UtilitiesManager.GetDateTimeAsString(zoneData.EED.Value);
                    string normalRateEED = UtilitiesManager.GetDateTimeAsString(zoneData.NormalRateToClose.CloseEffectiveDate);
                    context.Message = string.Format("EED '{0}' of the normal rate of zone '{1}' must be less than or equal to EED '{2}' of the zone", normalRateEED, zoneData.ZoneName, zoneEED);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            var zoneData = target as DataByZone;

            string zoneEED = UtilitiesManager.GetDateTimeAsString(zoneData.EED.Value);
            string normalRateEED = null;

            if (zoneData.NormalRateToChange != null)
            {
                if (zoneData.NormalRateToChange.EED.HasValue)
                    normalRateEED = UtilitiesManager.GetDateTimeAsString(zoneData.NormalRateToChange.EED.Value);
                else if (zoneData.NormalRateToClose != null)
                    normalRateEED = UtilitiesManager.GetDateTimeAsString(zoneData.NormalRateToClose.CloseEffectiveDate);
            }

            return String.Format("EED '{0}' of the normal rate of zone '{1}' must be less than or equal to EED '{2}' of the zone", normalRateEED, zoneData.ZoneName, zoneEED);
        }
    }
}
