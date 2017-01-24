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

			if (saleZones == null || saleZones.Count() == 0)
				return null;

			IEnumerable<long> applicableZoneIds = null;

			if (input.BulkAction != null)
			{
				if (input.BulkActionFilter == null)
					throw new Vanrise.Entities.DataIntegrityValidationException("BulkActionZoneFilter of BulkAction was not found");

				Changes draft = new RatePlanDraftManager().GetDraft(input.OwnerType, input.OwnerId);
				IEnumerable<long> saleZoneIds = saleZones.MapRecords(x => x.SaleZoneId);

				var applicableZoneIdsContext = new ApplicableZoneIdsContext()
				{
					OwnerType = input.OwnerType,
					OwnerId = input.OwnerId,
					BulkAction = input.BulkAction,
					DraftData = draft,
					SaleZones = saleZones
				};

				applicableZoneIds = input.BulkActionFilter.GetApplicableZoneIds(applicableZoneIdsContext);
			}

			Func<SaleZone, bool> filterFunc = (saleZone) =>
			{
				if (applicableZoneIds != null && !applicableZoneIds.Contains(saleZone.SaleZoneId))
					return false;
				if (!SaleZoneFilter(saleZone, input.CountryIds, input.ZoneNameFilterType, input.ZoneNameFilter))
					return false;
				return true;
			};

			return saleZones.MapRecords(x => char.ToUpper(x.Name[0]), filterFunc).Distinct().OrderBy(x => x);
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

			var changedCountryIds = new List<int>();
			if (draft != null && draft.CountryChanges != null && draft.CountryChanges.ChangedCountries != null && draft.CountryChanges.ChangedCountries.CountryIds != null)
				changedCountryIds.AddRange(draft.CountryChanges.ChangedCountries.CountryIds);

			int? sellingProductId = GetSellingProductId(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, false);
			if (sellingProductId == null)
				throw new Exception("Selling product does not exist");

			// TODO: Add EffectiveOn to input.Filter
			var effectiveOn = DateTime.Now.Date;

			var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
			var rateManager = new ZoneRateManager(input.Filter.OwnerType, input.Filter.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId, rateLocator);
			var rpManager = new ZoneRPManager(input.Filter.OwnerType, input.Filter.OwnerId, effectiveOn, draft);

			var baseRatesByZone = new BaseRatesByZone();
			var saleRateManager = new SaleRateManager();

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
					AddZoneBaseRates(baseRatesByZone, zoneItem); // Check if the customer zone has any inherited rate
					rpManager.SetCustomerZoneRP(zoneItem, input.Filter.OwnerId, sellingProductId.Value, zoneDraft);
				}

				zoneItem.IsCountryEnded = changedCountryIds.Contains(zoneItem.CountryId);
				zoneItems.Add(zoneItem);
			}

			if (input.Filter.OwnerType == SalePriceListOwnerType.Customer)
			{
				IEnumerable<DraftNewCountry> draftNewCountries = (draft != null && draft.CountryChanges != null) ? draft.CountryChanges.NewCountries : null;
				IEnumerable<CustomerCountry2> soldCountries = GetSoldCountries(input.Filter.OwnerId, effectiveOn, false, draftNewCountries);
				saleRateManager.ProcessBaseRatesByZone(input.Filter.OwnerId, baseRatesByZone, soldCountries);
			}

			//IEnumerable<RPZone> rpZones = zoneItems.MapRecords(x => new RPZone() { RoutingProductId = x.EffectiveRoutingProductId.Value, SaleZoneId = x.ZoneId }, x => x.EffectiveRoutingProductId.HasValue);
			//var routeOptionManager = new ZoneRouteOptionManager(input.Filter.OwnerType, input.Filter.OwnerId, input.Filter.RoutingDatabaseId, input.Filter.PolicyConfigId, input.Filter.NumberOfOptions, rpZones, input.Filter.CostCalculationMethods, input.Filter.CostCalculationMethodConfigId, input.Filter.RateCalculationMethod, input.CurrencyId);
			//routeOptionManager.SetZoneRouteOptionProperties(zoneItems);

			return zoneItems;
		}

		#region Zone Base Rate Methods

		private void AddZoneBaseRates(BaseRatesByZone baseRatesByZone, ZoneItem zoneItem)
		{
			if (zoneItem.IsCurrentRateEditable.HasValue && !zoneItem.IsCurrentRateEditable.Value)
			{
				int? rateTypeId = null;
				baseRatesByZone.AddZoneBaseRate(zoneItem.ZoneId, zoneItem, zoneItem.CountryId, rateTypeId, zoneItem.CurrentRateBED.Value, zoneItem.CurrentRateEED);
			}
			if (zoneItem.CurrentOtherRates != null)
			{
				foreach (OtherRate currentOtherRate in zoneItem.CurrentOtherRates.Values)
				{
					if (!currentOtherRate.IsRateEditable)
						baseRatesByZone.AddZoneBaseRate(zoneItem.ZoneId, zoneItem, zoneItem.CountryId, currentOtherRate.RateTypeId, currentOtherRate.BED, currentOtherRate.EED);
				}
			}
		}

		private IEnumerable<CustomerCountry2> GetSoldCountries(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<DraftNewCountry> draftNewCountries)
		{
			var allCountries = new List<CustomerCountry2>();
			var customerCountryManager = new CustomerCountryManager();

			IEnumerable<CustomerCountry2> soldCountries = customerCountryManager.GetCustomerCountries(customerId, effectiveOn, isEffectiveInFuture);
			if (soldCountries != null)
				allCountries.AddRange(soldCountries);

			if (draftNewCountries != null)
			{
				foreach (DraftNewCountry draftNewCountry in draftNewCountries)
				{
					allCountries.Add(new CustomerCountry2()
					{
						CustomerId = customerId,
						CountryId = draftNewCountry.CountryId,
						BED = draftNewCountry.BED,
						EED = draftNewCountry.EED
					});
				}
			}

			return allCountries;
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

			var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
			ZoneRateManager rateSetter = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId, rateLocator);
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

			var rpZones = new List<RPZone>();
			if (zoneItem.EffectiveRoutingProductId.HasValue)
			{
				rpZones.Add(new RPZone()
				{
					RoutingProductId = zoneItem.EffectiveRoutingProductId.Value,
					SaleZoneId = zoneItem.ZoneId
				});
			}

			var routeOptionManager = new ZoneRouteOptionManager(input.OwnerType, input.OwnerId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, rpZones, input.CostCalculationMethods, input.RateCalculationCostColumnConfigId, input.RateCalculationMethod, input.CurrencyId);
			routeOptionManager.SetZoneRouteOptionProperties(new List<ZoneItem>() { zoneItem });

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

		public IEnumerable<SaleZone> GetSaleZones(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, bool isEffectiveInFuture)
		{
			var saleZoneManager = new SaleZoneManager();
			int sellingNumberPlanId = GetOwnerSellingNumberPlanId(ownerType, ownerId);
			IEnumerable<SaleZone> soldSaleZones = saleZoneManager.GetSaleZonesByOwner(ownerType, ownerId, sellingNumberPlanId, effectiveOn, isEffectiveInFuture);

			IEnumerable<SaleZone> draftSoldSaleZones = null;
			if (ownerType == SalePriceListOwnerType.Customer)
				draftSoldSaleZones = GetCustomerDraftSaleZones(ownerId, sellingNumberPlanId, effectiveOn, isEffectiveInFuture);

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
			IEnumerable<int> newCountryIds = draft.CountryChanges.NewCountries.MapRecords(x => x.CountryId, x => x.IsEffectiveOrFuture(effectiveOn));
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

		#region Bulk Action Analysis Region

		public IEnumerable<ZoneItem> GetZoneItems(GetZoneItemsInput input)
		{
			// Get the sale zones
			IEnumerable<SaleZone> saleZones = GetSaleZones(input.OwnerType, input.OwnerId, input.EffectiveOn, true);
			if (saleZones == null)
				return null;

			Changes draft = new RatePlanDraftManager().GetDraft(input.OwnerType, input.OwnerId);

			// Filter and page the sale zones
			IEnumerable<long> applicableZoneIds = null;

			if (input.BulkAction != null)
			{
				if (input.Filter == null || input.Filter.BulkActionFilter == null)
					throw new Vanrise.Entities.MissingArgumentValidationException("input.Filter.BulkActionFilter");

				var applicableZoneContext = new ApplicableZoneIdsContext()
				{
					OwnerType = input.OwnerType,
					OwnerId = input.OwnerId,
					SaleZones = saleZones,
					DraftData = draft,
					BulkAction = input.BulkAction
				};
				applicableZoneIds = input.Filter.BulkActionFilter.GetApplicableZoneIds(applicableZoneContext);
			}

			Func<SaleZone, bool> filterFunc = (saleZone) =>
			{
				if (applicableZoneIds != null && !applicableZoneIds.Contains(saleZone.SaleZoneId))
					return false;
				if (!SaleZoneFilter(saleZone, input.Filter.CountryIds, input.Filter.ZoneNameFilterType, input.Filter.ZoneNameFilter))
					return false;
				if (char.ToLower(saleZone.Name.ElementAt(0)) != char.ToLower(input.Filter.ZoneLetter))
					return false;
				return true;
			};

			IEnumerable<SaleZone> filteredSaleZones = saleZones.FindAllRecords(filterFunc);
			IEnumerable<SaleZone> pagedSaleZones = GetPagedZones(filteredSaleZones, input.Filter.FromRow, input.Filter.ToRow);

			if (pagedSaleZones == null || pagedSaleZones.Count() == 0)
				return null;

			return BuildZoneItems(pagedSaleZones, input.OwnerType, input.OwnerId, input.CurrencyId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, input.CostCalculationMethods, input.BulkAction, draft, input.EffectiveOn);
		}

		public void ApplyBulkActionToDraft(ApplyActionToDraftInput input)
		{
			// Get the sale zones
			IEnumerable<SaleZone> saleZones = GetSaleZones(input.OwnerType, input.OwnerId, input.EffectiveOn, true);

			var ratePlanDraftManager = new RatePlanDraftManager();
			Changes draft = ratePlanDraftManager.GetDraft(input.OwnerType, input.OwnerId);

			// Filter the sale zones by applicable zones
			var applicableZoneIdsContext = new ApplicableZoneIdsContext()
			{
				OwnerType = input.OwnerType,
				OwnerId = input.OwnerId,
				SaleZones = saleZones,
				DraftData = draft,
				BulkAction = input.BulkAction
			};

			IEnumerable<long> applicableZoneIds = input.BulkActionFilter.GetApplicableZoneIds(applicableZoneIdsContext);

			Func<SaleZone, bool> filterFunc = (saleZone) =>
			{
				if (applicableZoneIds != null && !applicableZoneIds.Contains(saleZone.SaleZoneId))
					return false;
				return true;
			};

			IEnumerable<SaleZone> filteredSaleZones = saleZones.FindAllRecords(filterFunc);

			IEnumerable<ZoneChanges> existingZoneDrafts = new List<ZoneChanges>();
			if (draft != null && draft.ZoneChanges != null)
				existingZoneDrafts = draft.ZoneChanges.FindAllRecords(x => applicableZoneIds.Contains(x.ZoneId));

			Func<IEnumerable<ZoneItem>> buildZoneItems = () =>
			{
				return BuildZoneItems(filteredSaleZones, input.OwnerType, input.OwnerId, input.CurrencyId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, input.CostCalculationMethods, null, draft, input.EffectiveOn);
			};
			var applyBulkActionToDraftContext = new ApplyBulkActionToZoneDraftContext(buildZoneItems, input.CostCalculationMethods);

			var newDraft = new Changes();
			newDraft.CurrencyId = input.CurrencyId;
			newDraft.ZoneChanges = new List<ZoneChanges>();

			foreach (SaleZone zone in filteredSaleZones)
			{
				ZoneChanges zoneDraft = existingZoneDrafts.FindRecord(x => x.ZoneId == zone.SaleZoneId);
				if (zoneDraft == null)
				{
					zoneDraft = new ZoneChanges()
					{
						ZoneId = zone.SaleZoneId,
						ZoneName = zone.Name,
						CountryId = zone.CountryId
					};
				}
				newDraft.ZoneChanges.Add(zoneDraft);
				applyBulkActionToDraftContext.ZoneDraft = zoneDraft;
				input.BulkAction.ApplyBulkActionToZoneDraft(applyBulkActionToDraftContext);
			}

			ratePlanDraftManager.SaveDraft(input.OwnerType, input.OwnerId, newDraft);
		}

		public IEnumerable<ZoneItem> BuildZoneItems(IEnumerable<SaleZone> saleZones, SalePriceListOwnerType ownerType, int ownerId, int currencyId, int routingDatabaseId, Guid policyConfigId, int numberOfOptions, List<CostCalculationMethod> costCalculationMethods, BulkActionType bulkAction, Changes draft, DateTime effectiveOn)
		{
			var zoneItems = new List<ZoneItem>();

			var zoneDraftsByZone = new Dictionary<long, ZoneChanges>();
			var changedCountryIds = new List<int>();

			if (draft != null)
			{
				zoneDraftsByZone = SturctureZoneDraftsByZone(draft.ZoneChanges);

				if (draft.CountryChanges != null && draft.CountryChanges.ChangedCountries != null && draft.CountryChanges.ChangedCountries.CountryIds != null)
					changedCountryIds.AddRange(draft.CountryChanges.ChangedCountries.CountryIds);
			}

			int? sellingProductId = GetSellingProductId(ownerType, ownerId, DateTime.Now, false);
			if (sellingProductId == null)
				throw new Exception("Selling product does not exist");

			var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));
			var rateManager = new ZoneRateManager(ownerType, ownerId, sellingProductId, effectiveOn, draft, currencyId, rateLocator);
			var rpManager = new ZoneRPManager(ownerType, ownerId, effectiveOn, draft);
			ZoneRouteOptionManager routeOptionManager;

			var baseRatesByZone = new BaseRatesByZone(); // What the fuck to do with this?!
			var saleRateManager = new SaleRateManager();

			Dictionary<long, ZoneItem> contextZoneItemsByZone = null;

			Func<Dictionary<long, ZoneItem>> getContextZoneItems = () =>
			{
				if (contextZoneItemsByZone == null)
				{
					var contextZoneItems = new List<ZoneItem>();

					foreach (SaleZone saleZone in saleZones)
					{
						var contextZoneItem = new ZoneItem()
						{
							ZoneId = saleZone.SaleZoneId,
							ZoneName = saleZone.Name,
							CountryId = saleZone.CountryId,
							ZoneBED = saleZone.BED,
							ZoneEED = saleZone.EED
						};

						ZoneChanges zoneDraft = zoneDraftsByZone.GetRecord(saleZone.SaleZoneId);

						contextZoneItem.IsFutureZone = (contextZoneItem.ZoneBED.Date > DateTime.Now.Date);

						rateManager.SetZoneRate(contextZoneItem);

						if (ownerType == SalePriceListOwnerType.SellingProduct)
						{
							rpManager.SetSellingProductZoneRP(contextZoneItem, ownerId, zoneDraft);
						}
						else
						{
							AddZoneBaseRates(baseRatesByZone, contextZoneItem); // Check if the customer zone has any inherited rate
							rpManager.SetCustomerZoneRP(contextZoneItem, ownerId, sellingProductId.Value, zoneDraft);
						}

						contextZoneItem.IsCountryEnded = changedCountryIds.Contains(contextZoneItem.CountryId);
						contextZoneItems.Add(contextZoneItem);
					}

					if (ownerType == SalePriceListOwnerType.Customer)
					{
						IEnumerable<DraftNewCountry> draftNewCountries = (draft != null && draft.CountryChanges != null) ? draft.CountryChanges.NewCountries : null;
						IEnumerable<CustomerCountry2> soldCountries = GetSoldCountries(ownerId, effectiveOn, false, draftNewCountries);
						saleRateManager.ProcessBaseRatesByZone(ownerId, baseRatesByZone, soldCountries);
					}

					IEnumerable<RPZone> contextRPZones = contextZoneItems.MapRecords(x => new RPZone() { SaleZoneId = x.ZoneId, RoutingProductId = x.EffectiveRoutingProductId.Value }, x => x.EffectiveRoutingProductId.HasValue);
					routeOptionManager = new ZoneRouteOptionManager(ownerType, ownerId, routingDatabaseId, policyConfigId, numberOfOptions, contextRPZones, costCalculationMethods, null, null, currencyId);
					routeOptionManager.SetZoneRouteOptionProperties(contextZoneItems);

					contextZoneItemsByZone = contextZoneItems.ToDictionary(x => x.ZoneId);
				}

				return contextZoneItemsByZone;
			};

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

				ZoneChanges zoneDraft = zoneDraftsByZone.GetRecord(saleZone.SaleZoneId);

				if (bulkAction != null)
				{
					var applyBulkActionToZoneItemContext = new ApplyBulkActionToZoneItemContext(getContextZoneItems, costCalculationMethods)
					{
						ZoneItem = zoneItem,
						ZoneDraft = zoneDraft
					};
					bulkAction.ApplyBulkActionToZoneItem(applyBulkActionToZoneItemContext);
				}

				zoneItem.IsFutureZone = (zoneItem.ZoneBED.Date > DateTime.Now.Date);

				rateManager.SetZoneRate(zoneItem);

				// TODO: Refactor rateManager to handle products and customers separately
				if (ownerType == SalePriceListOwnerType.SellingProduct)
				{
					rpManager.SetSellingProductZoneRP(zoneItem, ownerId, zoneDraft);
				}
				else
				{
					AddZoneBaseRates(baseRatesByZone, zoneItem); // Check if the customer zone has any inherited rate
					rpManager.SetCustomerZoneRP(zoneItem, ownerId, sellingProductId.Value, zoneDraft);
				}

				zoneItem.IsCountryEnded = changedCountryIds.Contains(zoneItem.CountryId);
				zoneItems.Add(zoneItem);
			}

			if (ownerType == SalePriceListOwnerType.Customer)
			{
				IEnumerable<DraftNewCountry> draftNewCountries = (draft != null && draft.CountryChanges != null) ? draft.CountryChanges.NewCountries : null;
				IEnumerable<CustomerCountry2> soldCountries = GetSoldCountries(ownerId, effectiveOn, false, draftNewCountries);
				saleRateManager.ProcessBaseRatesByZone(ownerId, baseRatesByZone, soldCountries);
			}

			IEnumerable<RPZone> rpZones = zoneItems.MapRecords(x => new RPZone() { SaleZoneId = x.ZoneId, RoutingProductId = x.EffectiveRoutingProductId.Value }, x => x.EffectiveRoutingProductId.HasValue);
			routeOptionManager = new ZoneRouteOptionManager(ownerType, ownerId, routingDatabaseId, policyConfigId, numberOfOptions, rpZones, costCalculationMethods, null, null, currencyId);
			routeOptionManager.SetZoneRouteOptionProperties(zoneItems);

			return zoneItems;
		}

		private Dictionary<long, ZoneChanges> SturctureZoneDraftsByZone(IEnumerable<ZoneChanges> zoneDrafts)
		{
			var zoneDraftsByZone = new Dictionary<long, ZoneChanges>();
			if (zoneDrafts != null)
			{
				foreach (ZoneChanges zoneDraft in zoneDrafts)
				{
					if (!zoneDraftsByZone.ContainsKey(zoneDraft.ZoneId))
						zoneDraftsByZone.Add(zoneDraft.ZoneId, zoneDraft);
				}
			}
			return zoneDraftsByZone;
		}

		#endregion
	}
}
