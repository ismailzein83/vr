using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class SellingRuleCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            List<string> invalidZones = new List<string>();
            var ratePlanContext = context.GetExtension<IRatePlanContext>();
            ratePlanContext.ThrowIfNull("ratePlanContext");

            if (ratePlanContext.OwnerType != BusinessEntity.Entities.SalePriceListOwnerType.Customer)
            {
                return true;
            }
            var allDataByZone = context.Target as AllDataByZone;

            if (allDataByZone.DataByZoneList == null || allDataByZone.DataByZoneList.Count() == 0)
                return true;

            var ruleDefinitionId = new Guid("3D638C43-0191-464C-9E6E-CAAA5A2E2FDC");

            var sellingProductId = new TOne.WhS.BusinessEntity.Business.CarrierAccountManager().GetSellingProductId(ratePlanContext.OwnerId);

            var currentRateLocator = ratePlanContext.LastRateLocator;


            foreach (var zone in allDataByZone.DataByZoneList)
            {
                zone.ThrowIfNull("zone");
                var lastCustomerRate = currentRateLocator.GetCustomerZoneRate(ratePlanContext.OwnerId, sellingProductId, zone.ZoneId);
                lastCustomerRate.ThrowIfNull("lastCustomerRate");
                if (zone.NormalRateToChange != null)
                {
                    var thresholdContext = new ThresholdContext
                    {
                        CurrentRate = lastCustomerRate.Rate.Rate,
                        NewRate = zone.NormalRateToChange.NormalRate,
                        CurrentCurrencyId = zone.NormalRateToChange.CurrencyId.Value
                    };


                    var target = new Vanrise.GenericData.Entities.GenericRuleTarget
                    {
                        TargetFieldValues = new Dictionary<string, object>
                        {
                            {"SellingProduct", sellingProductId},
                            {"Customer", ratePlanContext.OwnerId},
                            {"SaleZone", zone.ZoneId}
                        }
                    };
                    target.EffectiveOn = zone.NormalRateToChange.BED;
                    new SellingRuleManager().ApplySellingRule(thresholdContext, ruleDefinitionId, target);
                    if (thresholdContext.ViolateRateRule)
                    {
                        invalidZones.Add(zone.ZoneName);
                    }
                }
            }

            if (invalidZones.Count > 0)
            {
                context.Message = string.Format("Zone (s) with new rate(s) that violates selling rule are : '{0}'", string.Join(",", invalidZones));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
