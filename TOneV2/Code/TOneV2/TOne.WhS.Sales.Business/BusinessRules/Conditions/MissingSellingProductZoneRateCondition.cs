using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class MissingSellingProductZoneRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return (target is DataByZone);
		}

		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

			if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
				return true;

			var zoneData = context.Target as DataByZone;

			var customerSellingProductManager = new CustomerSellingProductManager();
			int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(ratePlanContext.OwnerId, ratePlanContext.EffectiveDate, false);
			if (!sellingProductId.HasValue)
				throw new NullReferenceException("sellingProductId");

			SaleEntityZoneRate sellingProductRate = ratePlanContext.RateLocator.GetSellingProductZoneRate(sellingProductId.Value, zoneData.ZoneId);

			if (zoneData.NormalRateToClose != null && (sellingProductRate == null || sellingProductRate.Rate == null))
				return false;

			if (zoneData.OtherRatesToClose != null && zoneData.OtherRatesToClose.Count > 0)
			{
				if (sellingProductRate == null || sellingProductRate.RatesByRateType == null)
					return false;
				foreach (RateToClose otherRateToClose in zoneData.OtherRatesToClose)
				{
					if (!sellingProductRate.RatesByRateType.ContainsKey(otherRateToClose.RateTypeId.Value))
						return false;
				}
			}

			return true;
		}

		public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			var zoneData = target as DataByZone;
			return string.Format("Rate for Zone '{0}' of the assigned Selling Product was not found", zoneData.ZoneName);
		}
	}
}
