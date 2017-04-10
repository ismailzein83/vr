using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class CustomerZoneRateBEDIsGreaterThanSellingProductZoneRateBEDCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            //IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            //if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
            //    return true;

            //if (ratePlanContext.EffectiveAfterCustomerZoneRatesByZone == null || ratePlanContext.EffectiveAfterCustomerZoneRatesByZone.Count == 0)
            //    return true;

            //var zoneData = context.Target as DataByZone;

            //if (zoneData.NormalRateToChange != null)
            //{
            //    DateTime? maxNormalRateBED = ratePlanContext.EffectiveAfterCustomerZoneRatesByZone.GetMaxCustomerZoneRateBED(zoneData.ZoneId, null);

            //    if (maxNormalRateBED.HasValue && maxNormalRateBED.Value > zoneData.NormalRateToChange.BED)
            //    {
            //        context.Message = "Error";
            //        return false;
            //    }
            //}

            //if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
            //{
            //    foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
            //    {
            //        DateTime? maxOtherRateBED = ratePlanContext.EffectiveAfterCustomerZoneRatesByZone.GetMaxCustomerZoneRateBED(zoneData.ZoneId, otherRateToChange.RateTypeId);

            //        if (maxOtherRateBED.HasValue && maxOtherRateBED > otherRateToChange.BED)
            //        {
            //            context.Message = "Error";
            //            return false;
            //        }
            //    }
            //}

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return string.Empty;
        }
    }
}
