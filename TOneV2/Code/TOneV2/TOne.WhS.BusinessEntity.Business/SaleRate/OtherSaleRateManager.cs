﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
	public class OtherSaleRateManager
	{
		#region Fields

		private Vanrise.Common.Business.CurrencyManager _currencyManager = new Vanrise.Common.Business.CurrencyManager();

		#endregion

		public IEnumerable<SaleRateDetail> GetOtherSaleRates(OtherSaleRateQuery query)
		{
			IEnumerable<RateTypeInfo> allRateTypes = new Vanrise.Common.Business.RateTypeManager().GetAllRateTypes();
			if (allRateTypes == null || allRateTypes.Count() == 0)
				return null;

			IEnumerable<long> zoneIds = new List<long>() { query.ZoneId };
			return (query.OwnerType == SalePriceListOwnerType.SellingProduct) ? GetSellingProductZoneOtherRates(query, zoneIds, allRateTypes) : GetCustomerZoneOtherRates(query, zoneIds, query.ZoneId, allRateTypes);
		}

		#region Private Methods

		private IEnumerable<SaleRateDetail> GetSellingProductZoneOtherRates(OtherSaleRateQuery query, IEnumerable<long> zoneIds, IEnumerable<RateTypeInfo> allRateTypes)
		{
			var saleRates = new List<SaleRateDetail>();
			int currencyId = query.IsSystemCurrency ? new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId() : query.CurrencyId;

			var productZoneRateHistoryLocator = new ProductZoneRateHistoryLocator(new ProductZoneRateHistoryReader(CreateListFromItem(query.OwnerId), zoneIds, false, true));

			foreach (RateTypeInfo rateType in allRateTypes)
			{
				SaleRateHistoryRecord saleRateHistoryRecord = productZoneRateHistoryLocator.GetProductZoneRateHistoryRecord(query.OwnerId, query.ZoneName, rateType.RateTypeId, currencyId, query.EffectiveOn);
				if (saleRateHistoryRecord != null)
					AddSaleRate(saleRates, saleRateHistoryRecord, rateType, query.ZoneName, query.ZoneId, query.CountryId, currencyId, query.IsSystemCurrency);
			}

			return saleRates;
		}

		private IEnumerable<SaleRateDetail> GetCustomerZoneOtherRates(OtherSaleRateQuery query, IEnumerable<long> zoneIds, long zoneId, IEnumerable<RateTypeInfo> allRateTypes)
		{
			int longPrecisionValue = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
			int sellingProductId = new CarrierAccountManager().GetSellingProductId(query.OwnerId);
			var customerZoneRateHistoryLocatorV2 = new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(CreateListFromItem(query.OwnerId), CreateListFromItem(sellingProductId), zoneIds, false, true));

			IEnumerable<RateTypeInfo> customerZoneRateTypes = GetCustomerZoneRateTypes(query.OwnerId, zoneId, allRateTypes, query.EffectiveOn);
			int currencyId = query.IsSystemCurrency ? new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId() : query.CurrencyId;

			if (customerZoneRateTypes == null || customerZoneRateTypes.Count() == 0)
				return null;

			var saleRates = new List<SaleRateDetail>();

			foreach (RateTypeInfo rateType in customerZoneRateTypes)
			{
				SaleRateHistoryRecord saleRateHistoryRecord = customerZoneRateHistoryLocatorV2.GetCustomerZoneRateHistoryRecord(query.OwnerId, sellingProductId, query.ZoneName, rateType.RateTypeId, query.CountryId, query.EffectiveOn, currencyId, longPrecisionValue);
				if (saleRateHistoryRecord != null)
					AddSaleRate(saleRates, saleRateHistoryRecord, rateType, query.ZoneName, query.ZoneId, query.CountryId, currencyId, query.IsSystemCurrency);
			}

			return saleRates;
		}

		private IEnumerable<RateTypeInfo> GetCustomerZoneRateTypes(int customerId, long zoneId, IEnumerable<RateTypeInfo> allRateTypes, DateTime effectiveOn)
		{
			IEnumerable<int> rateTypeIds = Helper.GetRateTypeIds(customerId, zoneId, effectiveOn);
			return (rateTypeIds != null && rateTypeIds.Count() > 0) ? allRateTypes.FindAllRecords(x => rateTypeIds.Contains(x.RateTypeId)) : null;
		}

		private void AddSaleRate(List<SaleRateDetail> saleRates, SaleRateHistoryRecord saleRateHistoryRecord, RateTypeInfo rateType, string zoneName, long zoneId, int countryId, int currencyId, bool isSystemCurrency)
		{
			var saleRate = new SaleRateDetail();

			saleRate.Entity = new SaleRate()
			{
				SaleRateId = saleRateHistoryRecord.SaleRateId,
				RateTypeId = rateType.RateTypeId,
				ZoneId = zoneId,
				PriceListId = saleRateHistoryRecord.PriceListId,
				CurrencyId = saleRateHistoryRecord.CurrencyId, // CustomerZoneRateHistoryLocator gets the pricelist's currency if the rate's currency was not found
				Rate = saleRateHistoryRecord.Rate,
				BED = saleRateHistoryRecord.BED,
				EED = saleRateHistoryRecord.EED,
				SourceId = saleRateHistoryRecord.SourceId,
				RateChange = saleRateHistoryRecord.ChangeType
			};
			saleRate.ZoneName = zoneName;
			saleRate.CountryId = countryId;
			saleRate.RateTypeName = rateType.Name;
			saleRate.DisplayedCurrency = _currencyManager.GetCurrencySymbol(isSystemCurrency ? currencyId : saleRateHistoryRecord.CurrencyId);
			saleRate.DisplayedRate = isSystemCurrency ? saleRateHistoryRecord.ConvertedRate : saleRateHistoryRecord.Rate;
			saleRate.IsRateInherited = saleRateHistoryRecord.SellingProductId.HasValue;

			saleRates.Add(saleRate);
		}

		private IEnumerable<T> CreateListFromItem<T>(T item)
		{
			return new List<T>() { item };
		}

		#endregion
	}

}
