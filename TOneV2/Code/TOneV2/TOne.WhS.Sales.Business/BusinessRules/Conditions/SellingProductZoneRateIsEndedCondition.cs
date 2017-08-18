using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class SellingProductZoneRateIsEndedCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return (target is DataByZone);
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
            if (ratePlanContext.IsFirstSellingProductOffer.HasValue && ratePlanContext.IsFirstSellingProductOffer.Value)

                return true;
            if (ratePlanContext.OwnerType == TOne.WhS.BusinessEntity.Entities.SalePriceListOwnerType.Customer)
                return true;

            var zoneData = context.Target as DataByZone;

            if (zoneData.NormalRateToClose != null)
            {
                context.Message = string.Format("Zone '{0}' has a closed existing rate", zoneData.ZoneName);
                return false;
            }
            if (zoneData.OtherRatesToClose != null && zoneData.OtherRatesToClose.Count() > 0)
            {
                RateTypeManager rateTypeManager = new RateTypeManager();
                List<string> otherRateTypeNames = new List<string>();
                foreach (RateToClose rateToClose in zoneData.OtherRatesToClose)
                {
                    string otherRateTypeName = rateTypeManager.GetRateTypeName(rateToClose.RateTypeId.Value);
                    if (otherRateTypeName == null)
                        continue;
                    otherRateTypeNames.Add(otherRateTypeName);
                }
                if (otherRateTypeNames.Count > 0)
                {
                    string rateTypeNames = string.Join(", ", otherRateTypeNames);
                    context.Message = string.Format("Zone '{0}' has closed existing other rates of types '{1}'", zoneData.ZoneName, rateTypeNames);
                    return false;
                }
            }

            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.EED.HasValue)
            {
                context.Message = string.Format("Zone '{0}' has a new rate with an EED", zoneData.ZoneName);
                return false;
            }

            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count() > 0)
            {
                RateTypeManager rateTypeManager = new RateTypeManager();
                List<string> otherRateTypeNames = new List<string>();
                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    if (otherRateToChange.EED.HasValue)
                    {
                        string otherRateTypeName = rateTypeManager.GetRateTypeName(otherRateToChange.RateTypeId.Value);
                        if (otherRateTypeName == null)
                            continue;
                        otherRateTypeNames.Add(otherRateTypeName);
                    }
                }
                if (otherRateTypeNames.Count > 0)
                {
                    string rateTypeNames = string.Join(", ", otherRateTypeNames.ToArray());
                    context.Message = string.Format("Zone '{0}' has new other rates of types '{1}' with an EED", zoneData.ZoneName, rateTypeNames);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            var zoneData = target as DataByZone;
            return string.Format("Zone '{0}' either has new rates with an EED or closed existing rates", zoneData.ZoneName);
        }
    }
}
