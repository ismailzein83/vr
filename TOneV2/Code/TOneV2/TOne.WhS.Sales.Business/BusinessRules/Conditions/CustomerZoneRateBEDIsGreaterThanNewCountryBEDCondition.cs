using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
	public class CustomerZoneRateBEDIsGreaterThanNewCountryBEDCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
	{
		public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			return target is CountryData;
		}
		public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
		{
			IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

			if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
				return true;

			var countryData = context.Target as CountryData;

			if (!countryData.IsCountryNew)
				return true;

			if (countryData.ZoneDataByZoneId == null || countryData.ZoneDataByZoneId.Count == 0)
				return true;

			var invalidEffectiveZoneNames = new List<string>();
			var invalidFutureZoneNamesByZoneBED = new Dictionary<DateTime, List<string>>();

			foreach (DataByZone zoneData in countryData.ZoneDataByZoneId.Values)
			{
				DateTime validRateBED = Vanrise.Common.Utilities.Max(countryData.CountryBED, zoneData.BED);

				if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.BED != validRateBED)
				{
					AddInvalidZoneName(zoneData, invalidEffectiveZoneNames, invalidFutureZoneNamesByZoneBED);
					continue;
				}

				if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
				{
					var rateTypeIds = Helper.GetRateTypeIds(ratePlanContext.OwnerId, zoneData.ZoneId, DateTime.Now);
					foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
					{
						if (rateTypeIds.Contains(otherRateToChange.RateTypeId.Value) && otherRateToChange.BED != validRateBED)
						{
							AddInvalidZoneName(zoneData, invalidEffectiveZoneNames, invalidFutureZoneNamesByZoneBED);
							break;
						}
					}
				}
			}

			if (invalidEffectiveZoneNames.Count > 0 || invalidFutureZoneNamesByZoneBED.Count > 0)
			{
				var messageBuilder = new StringBuilder();

				string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryData.CountryId);
				string countryBEDString = countryData.CountryBED.ToString(ratePlanContext.DateFormat);
				if (invalidEffectiveZoneNames.Count > 0)
					messageBuilder.AppendFormat("Pricing zones of country '{0}' must be at the selling date '{1}', violated zone(s): ({2}). ", countryName, countryBEDString, string.Join(", ", invalidEffectiveZoneNames));

				if (invalidFutureZoneNamesByZoneBED.Count > 0)
				{
					foreach (KeyValuePair<DateTime, List<string>> kvp in invalidFutureZoneNamesByZoneBED)
					{
						string violateDateString;
						if (countryData.CountryBED > kvp.Key)
							violateDateString = string.Format("the selling date '{0}'", countryBEDString);
						else
						{
							string zoneBEDString = kvp.Key.ToString(ratePlanContext.DateFormat);
							violateDateString = string.Format("zones' BED '{0}'", zoneBEDString);
						}
						string invalidFutureZoneNamesString = string.Join(", ", kvp.Value);
						messageBuilder.AppendFormat("Pricing pending zones must be at '{0}', violated zone(s): ({1}). ", violateDateString, invalidFutureZoneNamesString);
					}
				}

				context.Message = messageBuilder.ToString();
				return false;
			}

			return true;
		}
		public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
		{
			throw new NotImplementedException();
		}

		#region Private Methods
		private void AddInvalidZoneName(DataByZone zoneData, List<string> invalidEffectiveZoneNames, Dictionary<DateTime, List<string>> invalidFutureZoneNamesByZoneBED)
		{
			if (zoneData.BED <= DateTime.Today)
				invalidEffectiveZoneNames.Add(zoneData.ZoneName);
			else
			{
				List<string> invalidFutureZoneNames = invalidFutureZoneNamesByZoneBED.GetOrCreateItem(zoneData.BED, () => { return new List<string>(); });
				invalidFutureZoneNames.Add(zoneData.ZoneName);
			}
		}
		#endregion
	}
}
