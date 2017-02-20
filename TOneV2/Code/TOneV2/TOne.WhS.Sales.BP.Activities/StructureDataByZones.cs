using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	public class StructureDataByZones : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<int> CurrencyId { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<SaleZoneRoutingProductToAdd>> SaleZoneRoutingProductsToAdd { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<SaleZoneRoutingProductToClose>> SaleZoneRoutingProductsToClose { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<SaleZoneServiceToAdd>> SaleZoneServicesToAdd { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<SaleZoneServiceToClose>> SaleZoneServicesToClose { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			IRatePlanContext ratePlanContext = context.GetRatePlanContext();
			SalePriceListOwnerType ownerType = ratePlanContext.OwnerType;
			int ownerId = ratePlanContext.OwnerId;
			int currencyId = CurrencyId.Get(context);

			IEnumerable<CustomerCountryToAdd> countriesToAdd = CustomerCountriesToAdd.Get(context);
			IEnumerable<CustomerCountryToChange> countriesToChange = CustomerCountriesToChange.Get(context);
			
			var endedCountryIds = new List<int>();
			if (countriesToChange != null)
				endedCountryIds.AddRange(countriesToChange.MapRecords(x => x.CountryId));

			IEnumerable<RateToChange> ratesToChange = this.RatesToChange.Get(context);
			IEnumerable<RateToClose> ratesToClose = this.RatesToClose.Get(context);

			IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = this.SaleZoneRoutingProductsToAdd.Get(context);
			IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = this.SaleZoneRoutingProductsToClose.Get(context);

			IEnumerable<SaleZoneServiceToAdd> saleZoneServicesToAdd = this.SaleZoneServicesToAdd.Get(context);
			IEnumerable<SaleZoneServiceToClose> saleZoneServicesToClose = this.SaleZoneServicesToClose.Get(context);

			IEnumerable<RatePlanCountry> soldCountries = null;
			if (ownerType == SalePriceListOwnerType.Customer)
				soldCountries = GetSoldCountries(ownerId, DateTime.Now.Date, false, countriesToAdd);

			var saleZoneManager = new SaleZoneManager();

			Dictionary<string, DataByZone> dataByZoneName = new Dictionary<string, DataByZone>();
			DataByZone dataByZone;

			var ratePlanManager = new RatePlanManager();
			var currencyExchangeManager = new CurrencyExchangeRateManager();
			var saleRateManager = new SaleRateManager();

			foreach (RateToChange rateToChange in ratesToChange)
			{
				if (!dataByZoneName.TryGetValue(rateToChange.ZoneName, out dataByZone))
					AddEmptyDataByZone(dataByZoneName, rateToChange.ZoneName, rateToChange.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

				if (rateToChange.RateTypeId.HasValue)
					dataByZone.OtherRatesToChange.Add(rateToChange);
				else
					dataByZone.NormalRateToChange = rateToChange;

				if (dataByZone.ZoneRateGroup == null)
					dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, rateToChange.ZoneId, DateTime.Now, currencyId, ratePlanManager, currencyExchangeManager, saleRateManager);

				if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
					dataByZone.SoldOn = GetStartEffectiveTime(soldCountries, saleZoneManager, rateToChange.ZoneId);
			}

			foreach (RateToClose rateToClose in ratesToClose)
			{
				if (!dataByZoneName.TryGetValue(rateToClose.ZoneName, out dataByZone))
					AddEmptyDataByZone(dataByZoneName, rateToClose.ZoneName, rateToClose.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

				if (rateToClose.RateTypeId.HasValue)
					dataByZone.OtherRatesToClose.Add(rateToClose);
				else
					dataByZone.NormalRateToClose = rateToClose;

				if (dataByZone.ZoneRateGroup == null)
					dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, rateToClose.ZoneId, DateTime.Now, currencyId, ratePlanManager, currencyExchangeManager, saleRateManager);

				if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
					dataByZone.SoldOn = GetStartEffectiveTime(soldCountries, saleZoneManager, rateToClose.ZoneId);
			}

			foreach (SaleZoneRoutingProductToAdd routingProductToAdd in saleZoneRoutingProductsToAdd)
			{
				if (!dataByZoneName.TryGetValue(routingProductToAdd.ZoneName, out dataByZone))
					AddEmptyDataByZone(dataByZoneName, routingProductToAdd.ZoneName, routingProductToAdd.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

                if (dataByZone.ZoneRateGroup == null)
                    dataByZone.ZoneRateGroup = GetZoneRateGroup(ownerType, ownerId, routingProductToAdd.ZoneId, DateTime.Now, currencyId, ratePlanManager, currencyExchangeManager, saleRateManager);

				dataByZone.SaleZoneRoutingProductToAdd = routingProductToAdd;
			}

			foreach (SaleZoneRoutingProductToClose routingProductToClose in saleZoneRoutingProductsToClose)
			{
				if (!dataByZoneName.TryGetValue(routingProductToClose.ZoneName, out dataByZone))
					AddEmptyDataByZone(dataByZoneName, routingProductToClose.ZoneName, routingProductToClose.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);
				dataByZone.SaleZoneRoutingProductToClose = routingProductToClose;
			}

			foreach (SaleZoneServiceToAdd saleZoneServiceToAdd in saleZoneServicesToAdd)
			{
				if (!dataByZoneName.TryGetValue(saleZoneServiceToAdd.ZoneName, out dataByZone))
					AddEmptyDataByZone(dataByZoneName, saleZoneServiceToAdd.ZoneName, saleZoneServiceToAdd.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

				dataByZone.SaleZoneServiceToAdd = saleZoneServiceToAdd;

				if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
					dataByZone.SoldOn = GetStartEffectiveTime(soldCountries, saleZoneManager, saleZoneServiceToAdd.ZoneId);
			}

			foreach (SaleZoneServiceToClose saleZoneServiceToClose in saleZoneServicesToClose)
			{
				if (!dataByZoneName.TryGetValue(saleZoneServiceToClose.ZoneName, out dataByZone))
					AddEmptyDataByZone(dataByZoneName, saleZoneServiceToClose.ZoneName, saleZoneServiceToClose.ZoneId, endedCountryIds, out dataByZone, saleZoneManager);

				dataByZone.SaleZoneServiceToClose = saleZoneServiceToClose;

				if (ownerType == SalePriceListOwnerType.Customer && !dataByZone.SoldOn.HasValue)
					dataByZone.SoldOn = GetStartEffectiveTime(soldCountries, saleZoneManager, saleZoneServiceToClose.ZoneId);
			}

			this.DataByZone.Set(context, dataByZoneName.Values);
		}

		#region Private Members

		private class RatePlanCountry
		{
			public int CountryId { get; set; }
			public DateTime BED { get; set; }
		}

		private RatePlanCountry CustomerCountryMapper(CustomerCountry2 customerCountry)
		{
			return new RatePlanCountry()
			{
				CountryId = customerCountry.CountryId,
				BED = customerCountry.BED
			};
		}

		private void AddEmptyDataByZone(Dictionary<string, DataByZone> dataByZoneName, string zoneName, long zoneId, IEnumerable<int> endedCountryIds, out DataByZone dataByZone, SaleZoneManager saleZoneManager)
		{
			int? countryId = saleZoneManager.GetSaleZoneCountryId(zoneId);
			if (!countryId.HasValue)
				throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Could not find the Country of Sale Zone '{0}'", zoneId));
			dataByZone = new DataByZone()
			{
				ZoneId = zoneId,
				ZoneName = zoneName,
				CountryId = countryId.Value,
				IsCountryEnded = endedCountryIds.Contains(countryId.Value),
				OtherRatesToChange = new List<RateToChange>(),
				OtherRatesToClose = new List<RateToClose>()
			};
			dataByZoneName.Add(zoneName, dataByZone);
		}

		private IEnumerable<RatePlanCountry> GetSoldCountries(int customerId, DateTime effectiveOn, bool isEffectiveInFuture, IEnumerable<CustomerCountryToAdd> countriesToAdd)
		{
			var customerCountryManager = new CustomerCountryManager();
			IEnumerable<CustomerCountry2> soldCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customerId, effectiveOn);

			IEnumerable<RatePlanCountry> draftCountries = null;
			if (countriesToAdd != null)
			{
				IEnumerable<int> newCountryIds = countriesToAdd.MapRecords(x => x.CountryId);
				draftCountries = countriesToAdd.MapRecords(x => new RatePlanCountry() { CountryId = x.CountryId, BED = x.BED });
			}

			var allCountries = new List<RatePlanCountry>();
			if (soldCountries != null) { allCountries.AddRange(soldCountries.MapRecords(CustomerCountryMapper)); }
			if (draftCountries != null) { allCountries.AddRange(draftCountries); }

			if (allCountries.Count == 0)
				throw new DataIntegrityValidationException(string.Format("No countries are sold to Customer '{0}'", customerId));

			return allCountries;
		}

		private DateTime GetStartEffectiveTime(IEnumerable<RatePlanCountry> soldCountries, SaleZoneManager saleZoneManager, long saleZoneId)
		{
			SaleZone saleZone = saleZoneManager.GetSaleZone(saleZoneId);
			if (saleZone == null)
				throw new NullReferenceException("saleZone");
			RatePlanCountry soldCountry = soldCountries.FindRecord(x => x.CountryId == saleZone.CountryId);
			if (soldCountry == null)
				throw new NullReferenceException("soldCountry");
			return soldCountry.BED;
		}

		private ZoneRateGroup GetZoneRateGroup(SalePriceListOwnerType ownerType, int ownerId, long zoneId, DateTime effectiveOn, int targetCurrencyId, RatePlanManager ratePlanManager, CurrencyExchangeRateManager currencyExchangeRateManager, SaleRateManager saleRateManager)
		{
			ZoneRateGroup zoneRateGroup = null;
			SaleEntityZoneRate currentRate = ratePlanManager.GetRate(ownerType, ownerId, zoneId, effectiveOn);
			if (currentRate != null)
			{
				zoneRateGroup = new ZoneRateGroup();
				if (currentRate.Rate != null)
				{
					zoneRateGroup.NormalRate = new ZoneRate();

					decimal convertedNormalRate =
						currencyExchangeRateManager.ConvertValueToCurrency(currentRate.Rate.Rate, saleRateManager.GetCurrencyId(currentRate.Rate), targetCurrencyId, effectiveOn);

					zoneRateGroup.NormalRate.Source = currentRate.Source;
					zoneRateGroup.NormalRate.Rate = convertedNormalRate;
					zoneRateGroup.NormalRate.BED = currentRate.Rate.BED;
					zoneRateGroup.NormalRate.EED = currentRate.Rate.EED;
				}
				if (currentRate.RatesByRateType != null)
				{
					zoneRateGroup.OtherRatesByType = new Dictionary<int, ZoneRate>();
					foreach (KeyValuePair<int, SaleRate> kvp in currentRate.RatesByRateType)
					{
						if (kvp.Value != null)
						{
							var otherRate = new ZoneRate();

							SalePriceListOwnerType otherRateSource;
							currentRate.SourcesByRateType.TryGetValue(kvp.Key, out otherRateSource);

							decimal convertedOtherRate =
								currencyExchangeRateManager.ConvertValueToCurrency(kvp.Value.Rate, saleRateManager.GetCurrencyId(kvp.Value), targetCurrencyId, effectiveOn);

							otherRate.Source = otherRateSource;
							otherRate.RateTypeId = kvp.Key;
							otherRate.Rate = convertedOtherRate;
							otherRate.BED = kvp.Value.BED;
							otherRate.EED = kvp.Value.EED;

							zoneRateGroup.OtherRatesByType.Add(kvp.Key, otherRate);
						}
					}
				}
			}
			return zoneRateGroup;
		}

		#endregion
	}
}
