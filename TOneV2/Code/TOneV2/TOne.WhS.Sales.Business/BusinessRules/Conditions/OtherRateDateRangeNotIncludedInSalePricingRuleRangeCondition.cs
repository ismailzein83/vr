﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
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
			var ratePlanContext = context.GetExtension<IRatePlanContext>();

			if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
				return true;

			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
			var allZoneData = context.Target as AllDataByZone;

			List<string> zoneWithInvalidBEDOrEED = new List<string>();
			foreach (var zoneData in allZoneData.DataByZoneList)
			{
				RateTypeRule matchRule;
				DateTime ruleBED = DateTime.MinValue;
				DateTime? ruleEED = null;

				if ((zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count() > 0) ||
					(zoneData.OtherRatesToClose != null && zoneData.OtherRatesToClose.Count() > 0))
				{
					matchRule = GetMatchRule(ratePlanContext.OwnerId, zoneData.ZoneId, DateTime.Now);
					matchRule.ThrowIfNull(string.Format("There is no match other rate type rule for zone '{0}' and customer '{1}'", zoneData.ZoneId, ratePlanContext.OwnerId));

					ruleBED = matchRule.BeginEffectiveTime.Date;
					if (matchRule.EndEffectiveTime.HasValue)
						ruleEED = matchRule.EndEffectiveTime.Value.Date;
				}

				if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count() > 0)
				{
					foreach (var otherRate in zoneData.OtherRatesToChange)
						if (otherRate.BED < ruleBED || ruleEED.VRLessThan(otherRate.BED))
						{
							zoneWithInvalidBEDOrEED.Add(zoneData.ZoneName);
							break;
						}
				}

				if (zoneData.OtherRatesToClose != null && zoneData.OtherRatesToClose.Count() > 0)
				{
					foreach (var otherRate in zoneData.OtherRatesToClose)
						if (otherRate.CloseEffectiveDate < ruleBED || ruleEED.VRLessThan(otherRate.CloseEffectiveDate))
						{
							zoneWithInvalidBEDOrEED.Add(zoneData.ZoneName);
							break;
						}
				}
			}

			if (zoneWithInvalidBEDOrEED.Count() > 0)
			{
				context.Message = string.Format("Some zone(s) are having other rates with a date which is not included in their Sale Pricing Rule date range. Violated zone(s): '{0}'", string.Join(",", zoneWithInvalidBEDOrEED));
				return false;
			}
			return true;
		}
		public override string GetMessage(IRuleTarget target)
		{
			throw new NotImplementedException();
		}
		private RateTypeRule GetMatchRule(int customerId, long zoneId, DateTime effectiveDate)
		{
            GenericRuleTarget target = Helper.PrepareRuleContext(customerId, zoneId, effectiveDate);
			return new RateTypeRuleManager().GetMatchRule(Helper._rateTypeRuleDefinitionId, target);
		}
	}
}


