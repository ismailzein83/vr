using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
	public class RatePlanManager
	{
		#region Fields

		IRatePlanDataManager _dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
		RoutingProductManager _routingProductManager = new RoutingProductManager();

		#endregion

		#region Public Methods

		public bool ValidateCustomer(int customerId, DateTime effectiveOn)
		{
			CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
			CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, false);

			return customerSellingProduct != null;
		}

		#region Get Zone Letters

		public IEnumerable<char> GetZoneLetters(ZoneLettersInput input)
		{
			IEnumerable<SaleZone> saleZones = GetSaleZones(input.OwnerType, input.OwnerId, input.EffectiveOn, true);

			if (saleZones == null)
				return null;
			else
			{
				IEnumerable<SaleZone> filteredSaleZones =
					saleZones.FindAllRecords(x => SaleZoneFilter(x, input.CountryIds, input.ZoneNameFilterType, input.ZoneNameFilter));
				return filteredSaleZones.MapRecords(x => char.ToUpper(x.Name[0])).Distinct().OrderBy(x => x);
			}
		}

		#endregion

		#region Get Zone Item(s)

		// TODO: Divide GetZoneItem(s) into GetSellingProductZoneItem(s) and GetCustomerZoneItem(s) to get rid of unnecessary sellingProductId == null and ownerType checks
		public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
		{
			IEnumerable<SaleZone> saleZones = GetSaleZones(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, true);
			if (saleZones == null)
				return null;

			// Filter the sale zones
			Func<SaleZone, bool> filterFunc = (saleZone) =>
			{
				if (!SaleZoneFilter(saleZone, input.Filter.CountryIds, input.Filter.ZoneNameFilterType, input.Filter.ZoneNameFilter))
					return false;
				if (char.ToLower(saleZone.Name.ElementAt(0)) != char.ToLower(input.Filter.ZoneLetter))
					return false;
				return true;
			};
			saleZones = saleZones.FindAllRecords(filterFunc);

			if (saleZones == null)
				return null;

			// Page the sale zones
			saleZones = GetPagedZones(saleZones, input.FromRow, input.ToRow);
			if (saleZones == null)
				return null;

			// Prepare the zone items
			var zoneItems = new List<ZoneItem>();
			Changes draft = _dataManager.GetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft);

			int? sellingProductId = GetSellingProductId(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, false);
			if (sellingProductId == null)
				throw new Exception("Selling product does not exist");

			// TODO: Add EffectiveOn to input.Filter
			var effectiveOn = DateTime.Now.Date;

			var rateManager = new ZoneRateManager(input.Filter.OwnerType, input.Filter.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId);
			var rpManager = new ZoneRPManager(input.Filter.OwnerType, input.Filter.OwnerId, effectiveOn, draft);

			var inheritedRatesByZone = new InheritedRatesByZone();

			foreach (SaleZone saleZone in saleZones)
			{
				var zoneItem = new ZoneItem()
				{
					ZoneId = saleZone.SaleZoneId,
					ZoneName = saleZone.Name,
					CountryId = saleZone.CountryId,
					ZoneBED = saleZone.BED,
					ZoneEED = saleZone.EED
				};

				zoneItem.IsFutureZone = (zoneItem.ZoneBED.Date > DateTime.Now.Date);

				ZoneChanges zoneDraft = null;
				if (draft != null && draft.ZoneChanges != null)
					zoneDraft = draft.ZoneChanges.FindRecord(x => x.ZoneId == saleZone.SaleZoneId);

				rateManager.SetZoneRate(zoneItem);

				// TODO: Refactor rateManager to handle products and customers separately
				if (input.Filter.OwnerType == SalePriceListOwnerType.SellingProduct)
				{
					rpManager.SetSellingProductZoneRP(zoneItem, input.Filter.OwnerId, zoneDraft);
				}
				else
				{
					AddZoneInheritedRates(inheritedRatesByZone, zoneItem); // Check if the customer zone has any inherited rate
					rpManager.SetCustomerZoneRP(zoneItem, input.Filter.OwnerId, sellingProductId.Value, zoneDraft);
				}

				zoneItems.Add(zoneItem);
			}

			if (input.Filter.OwnerType == SalePriceListOwnerType.Customer && inheritedRatesByZone.Count > 0)
			{
				SetZoneInheritedRateBEDs(input.Filter.OwnerId, effectiveOn, inheritedRatesByZone);
			}

			IEnumerable<RPZone> rpZones = zoneItems.MapRecords(x => new RPZone() { RoutingProductId = x.EffectiveRoutingProductId, SaleZoneId = x.ZoneId });
			var routeOptionManager = new ZoneRouteOptionManager(input.Filter.OwnerType, input.Filter.OwnerId, input.Filter.RoutingDatabaseId, input.Filter.PolicyConfigId, input.Filter.NumberOfOptions, rpZones, input.Filter.CostCalculationMethods, input.Filter.CostCalculationMethodConfigId, input.Filter.RateCalculationMethod, input.CurrencyId);
			routeOptionManager.SetZoneRouteOptionProperties(zoneItems);

			return zoneItems;
		}

		#region Zone Inherited Rate Methods

		private void AddZoneInheritedRates(InheritedRatesByZone inheritedRatesByZone, ZoneItem zoneItem)
		{
			if (zoneItem.IsCurrentRateEditable.HasValue && !zoneItem.IsCurrentRateEditable.Value)
			{
				AddZoneInheritedRate(inheritedRatesByZone, zoneItem, null, zoneItem.CurrentRateBED.Value, zoneItem.CurrentRateEED);
			}
			if (zoneItem.CurrentOtherRates != null)
			{
				foreach (OtherRate otherRate in zoneItem.CurrentOtherRates.Values)
				{
					if (!otherRate.IsRateEditable)
						AddZoneInheritedRate(inheritedRatesByZone, zoneItem, otherRate.RateTypeId, otherRate.BED, otherRate.EED);
				}
			}
		}

		private void AddZoneInheritedRate(InheritedRatesByZone inheritedRatesByZone, ZoneItem zoneItem, int? rateTypeId, DateTime rateBED, DateTime? rateEED)
		{
			ZoneInheritedRates zoneInheritedRates;
			if (!inheritedRatesByZone.TryGetValue(zoneItem.ZoneId, out zoneInheritedRates))
			{
				zoneInheritedRates = new ZoneInheritedRates()
				{
					ZoneItem = zoneItem,
					OtherRatesByType = new Dictionary<int, ZoneInheritedRate>()
				};
				inheritedRatesByZone.Add(zoneItem.ZoneId, zoneInheritedRates);
			}
			if (rateTypeId.HasValue)
			{
				if (!zoneInheritedRates.OtherRatesByType.ContainsKey(rateTypeId.Value))
				{
					zoneInheritedRates.OtherRatesByType.Add(rateTypeId.Value, new ZoneInheritedRate()
					{
						RateTypeId = rateTypeId.Value,
						BED = rateBED,
						EED = rateEED
					});
				}
			}
			else
			{
				zoneInheritedRates.NormalRate = new ZoneInheritedRate()
				{
					RateTypeId = rateTypeId,
					BED = rateBED,
					EED = rateEED
				};
			}
		}

		private void SetZoneInheritedRateBEDs(int customerId, DateTime effectiveOn, InheritedRatesByZone inheritedRatesByZone)
		{
			var saleZoneManager = new SaleZoneManager();

			IEnumerable<long> zoneIds = inheritedRatesByZone.Values.MapRecords(x => x.ZoneItem.ZoneId);
			DateTime minimumBED = inheritedRatesByZone.GetMinimumBED();

			OverlappedRatesByZone overlappedRatesByZone = new SaleRateManager().GetCustomerOverlappedRatesByZone(customerId, zoneIds, minimumBED);
			Dictionary<int, DateTime> countryIdsBySoldOn = GetCountryIdsBySoldOn(customerId, effectiveOn);

			foreach (ZoneInheritedRates zoneInheritedRates in inheritedRatesByZone.Values)
			{
				ZoneOverlappedRates zoneOverlappedRates = overlappedRatesByZone.GetRecord(zoneInheritedRates.ZoneItem.ZoneId);
				DateTime soldOn = countryIdsBySoldOn.GetRecord(zoneInheritedRates.ZoneItem.CountryId);

				if (zoneInheritedRates.NormalRate != null)
				{
					zoneInheritedRates.ZoneItem.CurrentRateBED = saleZoneManager.GetCustomerInheritedZoneRateBED(null, zoneOverlappedRates, zoneInheritedRates.NormalRate.BED, zoneInheritedRates.NormalRate.EED, soldOn);
				}

				if (zoneInheritedRates.OtherRatesByType.Count > 0)
				{
					foreach (ZoneInheritedRate inheritedOtherRate in zoneInheritedRates.OtherRatesByType.Values)
					{
						OtherRate currentOtherRate = zoneInheritedRates.ZoneItem.CurrentOtherRates.GetRecord(inheritedOtherRate.RateTypeId.Value);
						currentOtherRate.BED = saleZoneManager.GetCustomerInheritedZoneRateBED(inheritedOtherRate.RateTypeId.Value, zoneOverlappedRates, inheritedOtherRate.BED, inheritedOtherRate.EED, soldOn);
					}
				}
			}
		}

		private Dictionary<int, DateTime> GetCountryIdsBySoldOn(int customerId, DateTime effectiveOn)
		{
			var countryIdsBySoldOn = new Dictionary<int, DateTime>();

			IEnumerable<CustomerCountry2> soldCountries = new CustomerCountryManager().GetCustomerCountries(customerId, effectiveOn, false);

			Changes draft = new RatePlanDraftManager().GetDraft(SalePriceListOwnerType.Customer, customerId);
			IEnumerable<DraftNewCountry> newCountries = (draft != null && draft.CountryChanges != null) ? draft.CountryChanges.NewCountries : null;

			if ((soldCountries == null || soldCountries.Count() == 0) && (newCountries == null || newCountries.Count() == 0))
				throw new Vanrise.Entities.DataIntegrityValidationException("countries");

			if (soldCountries != null)
			{
				foreach (CustomerCountry2 soldCountry in soldCountries)
				{
					if (!countryIdsBySoldOn.ContainsKey(soldCountry.CountryId))
						countryIdsBySoldOn.Add(soldCountry.CountryId, soldCountry.BED);
				}
			}

			if (newCountries != null)
			{
				foreach (DraftNewCountry newCountry in newCountries)
				{
					if (!countryIdsBySoldOn.ContainsKey(newCountry.CountryId))
						countryIdsBySoldOn.Add(newCountry.CountryId, newCountry.BED);
				}
			}

			return (countryIdsBySoldOn.Count > 0) ? countryIdsBySoldOn : null;
		}

		#endregion

		private IEnumerable<SaleZone> GetPagedZones(IEnumerable<SaleZone> saleZones, int fromRow, int toRow)
		{
			if (saleZones == null)
				return saleZones;

			List<SaleZone> pagedZones = null;

			saleZones = saleZones.OrderBy(x => x.Name);
			int count = saleZones.Count();

			if (count >= fromRow)
			{
				pagedZones = new List<SaleZone>();

				for (int i = fromRow - 1; i < count && i < toRow; i++)
					pagedZones.Add(saleZones.ElementAt(i));
			}

			return pagedZones;
		}

		public ZoneItem GetZoneItem(ZoneItemInput input)
		{
			SaleZoneManager saleZoneManager = new SaleZoneManager();

			SaleZone saleZone = saleZoneManager.GetSaleZone(input.ZoneId);
			if (saleZone == null)
				throw new NullReferenceException(string.Format("SaleZone '{0}' was not found", input.ZoneId));

			ZoneItem zoneItem = new ZoneItem()
			{
				ZoneId = input.ZoneId,
				ZoneName = saleZone.Name,
				CountryId = saleZone.CountryId
			};

			// TODO: Add EffectiveOn to input
			var effectiveOn = DateTime.Now.Date;

			int? sellingProductId = GetSellingProductId(input.OwnerType, input.OwnerId, effectiveOn, false);
			Changes draft = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);

			ZoneRateManager rateSetter = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId);
			rateSetter.SetZoneRate(zoneItem);

			if (sellingProductId == null)
				throw new Exception("Selling product does not exist");

			int? customerId = null;
			if (input.OwnerType == SalePriceListOwnerType.Customer)
				customerId = input.OwnerId;

			var rpManager = new ZoneRPManager(input.OwnerType, input.OwnerId, effectiveOn, draft);

			ZoneChanges zoneDraft = null;
			if (draft != null && draft.ZoneChanges != null)
				zoneDraft = draft.ZoneChanges.FindRecord(x => x.ZoneId == input.ZoneId);

			if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
				rpManager.SetSellingProductZoneRP(zoneItem, input.OwnerId, zoneDraft);
			else
				rpManager.SetCustomerZoneRP(zoneItem, input.OwnerId, sellingProductId.Value, zoneDraft);

			RPZone rpZone = new RPZone() { RoutingProductId = zoneItem.EffectiveRoutingProductId, SaleZoneId = zoneItem.ZoneId };
			ZoneRouteOptionManager routeOptionSetter = new ZoneRouteOptionManager(input.OwnerType, input.OwnerId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, new List<RPZone>() { rpZone }, input.CostCalculationMethods, input.RateCalculationCostColumnConfigId, input.RateCalculationMethod, input.CurrencyId);
			routeOptionSetter.SetZoneRouteOptionProperties(new List<ZoneItem>() { zoneItem });

			return zoneItem;
		}

		#endregion

		public bool SyncImportedDataWithDB(long processInstanceId, int? salePriceListId, SalePriceListOwnerType ownerType, int ownerId, int currencyId, DateTime effectiveOn)
		{
			var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
			return ratePlanDataManager.SyncImportedDataWithDB(processInstanceId, salePriceListId, ownerType, ownerId, currencyId, effectiveOn);
		}

		public int GetOwnerSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
		{
			int? sellingNumberPlanId = (ownerType == SalePriceListOwnerType.SellingProduct) ?
				new SellingProductManager().GetSellingNumberPlanId(ownerId) :
				new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
			if (!sellingNumberPlanId.HasValue)
				throw new NullReferenceException("sellingNumberPlanId");
			return sellingNumberPlanId.Value;
		}

		public RatePlanSettingsData GetRatePlanSettingsData()
		{
			object ratePlanSettingData = GetSettingData(Sales.Business.Constants.RatePlanSettingsType);
			var ratePlanSettings = ratePlanSettingData as RatePlanSettingsData;
			if (ratePlanSettings == null)
				throw new NullReferenceException("ratePlanSettings");
			return ratePlanSettings;
		}

		public SaleAreaSettingsData GetSaleAreaSettingsData()
		{
			object saleAreaSettingData = GetSettingData(BusinessEntity.Business.Constants.SaleAreaSettings);
			var saleAreaSettings = saleAreaSettingData as SaleAreaSettingsData;
			if (saleAreaSettings == null)
				throw new NullReferenceException("saleAreaSettings");
			return saleAreaSettings;
		}

		private object GetSettingData(string settingType)
		{
			var settingManager = new SettingManager();
			Setting setting = settingManager.GetSettingByType(settingType);
			if (setting == null)
				throw new NullReferenceException("setting");
			if (setting.Data == null)
				throw new NullReferenceException("setting.Data");
			return setting.Data;
		}

		public int? GetSellingProductId(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, bool isEffectiveInFuture)
		{
			if (ownerType == SalePriceListOwnerType.Customer)
			{
				CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
				return customerSellingProductManager.GetEffectiveSellingProductId(ownerId, effectiveOn, isEffectiveInFuture); // Review what isEffectiveFuture does
			}
			else // If the owner is a selling product
				return ownerId;
		}

		public int GetSellingProductId(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
		{
			var customerSellingProductManager = new CustomerSellingProductManager();
			int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, isEffectiveInFuture);
			if (!sellingProductId.HasValue)
				throw new NullReferenceException(String.Format("Customer '{0}' is not assigned to a SellingProduct", customerId));
			return sellingProductId.Value;
		}

		public SaleEntityZoneRate GetRate(SalePriceListOwnerType ownerType, int ownerId, long zoneId, DateTime effectiveOn)
		{
			var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));

			if (ownerType == SalePriceListOwnerType.SellingProduct)
				return rateLocator.GetSellingProductZoneRate(ownerId, zoneId);
			else
			{
				int? sellingProductId = GetSellingProductId(ownerType, ownerId, effectiveOn, false);
				if (sellingProductId == null)
					throw new NullReferenceException("sellingProductId");
				return rateLocator.GetCustomerZoneRate(ownerId, sellingProductId.Value, zoneId);
			}
		}

		#endregion

		#region Private Methods

		public IEnumerable<SaleZone> GetSaleZones(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, bool includeFutureZones)
		{
			var saleZoneManager = new SaleZoneManager();
			int sellingNumberPlanId = GetOwnerSellingNumberPlanId(ownerType, ownerId);
			IEnumerable<SaleZone> soldSaleZones = saleZoneManager.GetSaleZonesByOwner(ownerType, ownerId, sellingNumberPlanId, effectiveOn, includeFutureZones);

			IEnumerable<SaleZone> draftSoldSaleZones = null;
			if (ownerType == SalePriceListOwnerType.Customer)
				draftSoldSaleZones = GetCustomerDraftSaleZones(ownerId, sellingNumberPlanId, effectiveOn, includeFutureZones);

			var ownerSaleZones = new List<SaleZone>();
			if (soldSaleZones != null) { ownerSaleZones.AddRange(soldSaleZones); }
			if (draftSoldSaleZones != null) { ownerSaleZones.AddRange(draftSoldSaleZones); }

			return (ownerSaleZones.Count > 0) ? ownerSaleZones : null;
		}
		private IEnumerable<SaleZone> GetCustomerDraftSaleZones(int customerId, int sellingNumberPlanId, DateTime effectiveOn, bool includeFutureZones)
		{
			var draftManager = new RatePlanDraftManager();
			Changes draft = draftManager.GetDraft(SalePriceListOwnerType.Customer, customerId);

			if (draft == null || draft.CountryChanges == null || draft.CountryChanges.NewCountries == null)
				return null;

			var saleZoneManager = new SaleZoneManager();
			IEnumerable<int> newCountryIds = draft.CountryChanges.NewCountries.MapRecords(x => x.CountryId, x => x.IsEffective(effectiveOn));
			return saleZoneManager.GetSaleZonesByCountryIds(sellingNumberPlanId, newCountryIds, effectiveOn, includeFutureZones);
		}
		public bool SaleZoneFilter(SaleZone saleZone, IEnumerable<int> countryIds, Vanrise.Entities.TextFilterType? zoneNameFilterType, string zoneNameFilter)
		{
			if (countryIds != null && !countryIds.Contains(saleZone.CountryId))
				return false;
			if (saleZone.Name == null || saleZone.Name.Length == 0)
				return false;
			if (zoneNameFilterType.HasValue && !Vanrise.Common.Utilities.IsTextMatched(saleZone.Name, zoneNameFilter, zoneNameFilterType.Value))
				return false;
			return true;
		}

		#endregion
	}
}
