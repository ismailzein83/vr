using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
	public class NormalRateBEDCondition : BusinessRuleCondition
	{
		public override bool ShouldValidate(IRuleTarget target)
		{
			return target is AllDataByZone;
		}
		public override bool Validate(IBusinessRuleConditionValidateContext context)
		{
			var ratePlanContext = context.GetExtension<IRatePlanContext>();

			if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
			{
				CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

				int sellingProductId = carrierAccountManager.GetSellingProductId(ratePlanContext.OwnerId);
				var lastRateLocator = ratePlanContext.LastRateLocator;

				var allZoneData = context.Target as AllDataByZone;

				List<string> zoneWithNormalBEDLessThanOtherRatesBED = new List<string>();

				foreach (var zoneData in allZoneData.DataByZoneList)
				{
					if (zoneData.NormalRateToChange != null)
					{
						var lastCustomerRate = lastRateLocator.GetCustomerZoneRate(ratePlanContext.OwnerId, sellingProductId, zoneData.ZoneId);

						if (lastCustomerRate == null)
							throw new VRBusinessException(string.Format("Zone {0} has no rates set neither for customer nor for selling product", zoneData.ZoneName));
						if (lastCustomerRate.RatesByRateType != null && lastCustomerRate.RatesByRateType.Values.Count > 0)
						{
							var max = lastCustomerRate.RatesByRateType.Values.MaxBy(item => item.BED).BED;
							if (zoneData.NormalRateToChange.BED < max)
								zoneWithNormalBEDLessThanOtherRatesBED.Add(zoneData.ZoneName);
						}
					}
				}

				if (zoneWithNormalBEDLessThanOtherRatesBED.Count > 0)
				{
					context.Message = string.Format("Zone(s) '{0}' are having new normal rate with BED less than the BED of existing other rates", string.Join(",", zoneWithNormalBEDLessThanOtherRatesBED));
					return false;
				}
			}
			return true;
		}
		public override string GetMessage(IRuleTarget target)
		{
			throw new NotImplementedException();
		}
	}
}


