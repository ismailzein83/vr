using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Pricing;

namespace TOne.WhS.Sales.Business
{
    public class OtherRateDateRangeNotIncludedInSalePricingRuleRangeCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            return true;
            var ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            //TODO: Throw a new data integrity validation exception

            // if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
            //{
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var allZoneData = context.Target as AllDataByZone;

            List<string> zoneWithInvalidBEDOrEED = new List<string>();
            foreach (var zoneData in allZoneData.DataByZoneList)
            {
                if (zoneData.OtherRatesToChange.Count() > 0)
                {
                    RateTypeRule matchRule = GetMatchRule(ratePlanContext.OwnerId, zoneData.ZoneId, DateTime.Now);
                    if (zoneData.OtherRatesToChange.First().BED <= matchRule.BeginEffectiveTime && zoneData.OtherRatesToChange.First().EED >= matchRule.EndEffectiveTime)
                    {
                        zoneWithInvalidBEDOrEED.Add(zoneData.ZoneName);
                    }
                }
            }

            if (zoneWithInvalidBEDOrEED.Count() > 0)
            {
                context.Message = string.Format("Zone(s) '{0}' are having other rates with a date which is not included in their Sale Pricing Rule date", string.Join(",", zoneWithInvalidBEDOrEED));
                return false;
            }
            // }
            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
        private RateTypeRule GetMatchRule(int customerId, long zoneId, DateTime effectiveDate)
        {
            var ruleDefinitionId = new Guid("8A637067-0056-4BAE-B4D5-F80F00C0141B");
            var target = new Vanrise.GenericData.Entities.GenericRuleTarget
            {
                TargetFieldValues = new Dictionary<string, object>
                {
                    {"CustomerId", customerId},
                    {"SaleZoneId", zoneId}
                }
            };
            target.EffectiveOn = effectiveDate;
            return new RateTypeRuleManager().GetMatchRule(ruleDefinitionId, target);
        }
    }
}


