using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Business
{
	public class DealDefinitionManager : BaseDealManager
	{
		#region Public Methods

		public IEnumerable<DealDefinitionInfo> GetDealDefinitionInfo(DealDefinitionFilter filter)
		{
			var cachedDeals = base.GetCachedDeals();

			Func<DealDefinition, bool> filterExpression = (dealDefinition) =>
			{
				if (filter == null)
					return true;

				if (filter.IncludedDealDefinitionIds != null && !filter.IncludedDealDefinitionIds.Contains(dealDefinition.DealId))
					return false;

				if (filter.ExcludedDealDefinitionIds != null && filter.ExcludedDealDefinitionIds.Contains(dealDefinition.DealId))
					return false;

				if (filter.Filters != null)
				{
					DealDefinitionFilterContext context = new DealDefinitionFilterContext() { DealDefinition = dealDefinition };
					foreach (IDealDefinitionFilter dealDefinitionFilter in filter.Filters)
					{
						if (!dealDefinitionFilter.IsMatched(context))
							return false;
					}
				}

				return true;
			};

			return cachedDeals.MapRecords(DealDefinitionInfoMapper, filterExpression).OrderBy(item => item.Name);
		}

		/// <summary>
		/// used in data transformation
		/// </summary>
		/// <param name="record"></param>
		public void FillOrigCDRValues(dynamic record)
		{
			record.OrigSaleRateId = record.SaleRateId;
			record.OrigSaleRateValue = record.SaleRateValue;
			record.OrigSaleNet = record.SaleNet;
			record.OrigSaleExtraChargeRateValue = record.SaleExtraChargeRateValue;
			record.OrigSaleExtraChargeValue = record.SaleExtraChargeValue;
			record.OrigSaleDurationInSeconds = record.SaleDurationInSeconds;
			record.OrigSaleCurrencyId = record.SaleCurrencyId;

			record.OrigCostRateId = record.CostRateId;
			record.OrigCostRateValue = record.CostRateValue;
			record.OrigCostNet = record.CostNet;
			record.OrigCostExtraChargeRateValue = record.CostExtraChargeRateValue;
			record.OrigCostExtraChargeValue = record.CostExtraChargeValue;
			record.OrigCostDurationInSeconds = record.CostDurationInSeconds;
			record.OrigCostCurrencyId = record.CostCurrencyId;
		}

		public void FillOrigSaleDealValues(dynamic record)
		{
			DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetAccountSaleZoneGroup(record.CustomerId, record.SaleZoneId, record.AttemptDateTime);
			if (dealSaleZoneGroup != null)
			{
				record.OrigSaleDealId = dealSaleZoneGroup.DealId;
				record.OrigSaleDealZoneGroupNb = dealSaleZoneGroup.DealSaleZoneGroupNb;
			}
			else
			{
				record.OrigSaleDealId = null;
				record.OrigSaleDealZoneGroupNb = null;
			}
		}

		public void FillOrigCostDealValues(dynamic record)
		{
			DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup = GetAccountSupplierZoneGroup(record.SupplierId, record.SupplierZoneId, record.AttemptDateTime);
			if (dealSupplierZoneGroup != null)
			{
				record.OrigCostDealId = dealSupplierZoneGroup.DealId;
				record.OrigCostDealZoneGroupNb = dealSupplierZoneGroup.DealSupplierZoneGroupNb;
			}
			else
			{
				record.OrigCostDealId = null;
				record.OrigCostDealZoneGroupNb = null;
			}
		}

		public DealSaleZoneGroupWithoutRate GetAccountSaleZoneGroup(int? customerId, long? saleZoneId, DateTime effectiveDate)
		{
			if (!customerId.HasValue || !saleZoneId.HasValue)
				return null;

			var cachedAccountSaleZoneGroups = GetCachedAccountSaleZoneGroups();
			IOrderedEnumerable<DealSaleZoneGroupWithoutRate> dealSaleZoneGroups = cachedAccountSaleZoneGroups.GetRecord(new AccountZoneGroup() { AccountId = customerId.Value, ZoneId = saleZoneId.Value });
			if (dealSaleZoneGroups != null && dealSaleZoneGroups.Count() > 0)
			{
				foreach (DealSaleZoneGroupWithoutRate dealSaleZoneGroup in dealSaleZoneGroups)
				{
					if (dealSaleZoneGroup.Zones == null)
						continue;

					if (dealSaleZoneGroup.IsEffective(effectiveDate)
					&& dealSaleZoneGroup.Zones.Any(item => item.ZoneId == saleZoneId && item.IsEffective(effectiveDate)))
						return dealSaleZoneGroup;
				}
			}
			return null;
		}

		public DealSupplierZoneGroupWithoutRate GetAccountSupplierZoneGroup(int? supplierId, long? supplierZoneId, DateTime effectiveDate)
		{
			if (!supplierId.HasValue || !supplierZoneId.HasValue)
				return null;

			var cachedAccountSupplierZoneGroups = GetCachedAccountSupplierZoneGroups();
			IOrderedEnumerable<DealSupplierZoneGroupWithoutRate> dealSupplierZoneGroups = cachedAccountSupplierZoneGroups.GetRecord(new AccountZoneGroup() { AccountId = supplierId.Value, ZoneId = supplierZoneId.Value });
			if (dealSupplierZoneGroups != null && dealSupplierZoneGroups.Count() > 0)
			{
				foreach (DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup in dealSupplierZoneGroups)
				{
					if (dealSupplierZoneGroup.Zones == null)
						continue;

					if (dealSupplierZoneGroup.IsEffective(effectiveDate)
					&& dealSupplierZoneGroup.Zones.Any(item => item.ZoneId == supplierZoneId && item.IsEffective(effectiveDate)))
						return dealSupplierZoneGroup;
				}
			}
			return null;
		}

		public DealZoneGroupTierDetailsWithoutRate GetDealZoneGroupTierDetailsWithoutRate(bool isSale, int dealId, int zoneGroupNb, int tierNb)
		{
			if (isSale)
				return GetSaleDealZoneGroupTierDetailsWithoutRate(dealId, zoneGroupNb, tierNb);
			else
				return GetSupplierDealZoneGroupTierDetailsWithoutRate(dealId, zoneGroupNb, tierNb);
		}

		public DealZoneGroupTierDetailsWithoutRate GetSaleDealZoneGroupTierDetailsWithoutRate(int dealId, int zoneGroupNb, int tierNb)
		{
			DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetDealSaleZoneGroup(dealId, zoneGroupNb);
			if (dealSaleZoneGroup == null || dealSaleZoneGroup.Tiers == null)
				return null;

			DealSaleZoneGroupTierWithoutRate dealZoneGroupTier = dealSaleZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
			if (dealZoneGroupTier == null)
				return null;

			return BuildDealZoneGroupTierDetailsWithoutRate(dealZoneGroupTier);
		}

		public DealZoneGroupTierDetailsWithoutRate GetSupplierDealZoneGroupTierDetailsWithoutRate(int dealId, int zoneGroupNb, int tierNb)
		{
			DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup = GetDealSupplierZoneGroupWithoutRate(dealId, zoneGroupNb);
			if (dealSupplierZoneGroup == null || dealSupplierZoneGroup.Tiers == null)
				return null;

			DealSupplierZoneGroupTierWithoutRate dealZoneGroupTier = dealSupplierZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
			if (dealZoneGroupTier == null)
				return null;

			return BuildDealZoneGroupTierDetailsWithoutRate(dealZoneGroupTier);
		}

		public DealZoneGroupTierDetails GetSaleDealZoneGroupTierDetails(int dealId, int zoneGroupNb, int tierNb, long zoneId, DateTime effectiveTime)
		{
			DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetDealSaleZoneGroup(dealId, zoneGroupNb);
			if (dealSaleZoneGroup == null || dealSaleZoneGroup.Tiers == null)
				return null;

			DealSaleZoneGroupTierWithoutRate dealZoneGroupTier = dealSaleZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
			if (dealZoneGroupTier == null)
				return null;

			var dealZoneRate = new DealZoneRateManager().GetDealZoneRate(dealId, zoneGroupNb, zoneId, tierNb, true, effectiveTime);
			if (dealZoneRate == null)
				return null;

			return BuildDealZoneGroupTierDetails(dealZoneGroupTier, dealZoneRate.Rate, dealZoneRate.CurrencyId);
		}

		public DealZoneGroupTierDetails GetSupplierDealZoneGroupTierDetails(int dealId, int zoneGroupNb, int tierNb, long zoneId, DateTime effectiveTime)
		{
			DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup = GetDealSupplierZoneGroupWithoutRate(dealId, zoneGroupNb);
			if (dealSupplierZoneGroup == null || dealSupplierZoneGroup.Tiers == null)
				return null;

			DealSupplierZoneGroupTierWithoutRate dealZoneGroupTier = dealSupplierZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
			if (dealZoneGroupTier == null)
				return null;

			var dealZoneRate = new DealZoneRateManager().GetDealZoneRate(dealId, zoneGroupNb, zoneId, tierNb, false, effectiveTime);
			if (dealZoneRate == null)
				return null;

			return BuildDealZoneGroupTierDetails(dealZoneGroupTier, dealZoneRate.Rate, dealZoneRate.CurrencyId);
		}

		public override BaseDealManager.BaseDealLoggableEntity GetLoggableEntity()
		{
			throw new NotImplementedException();
		}

		public Dictionary<int, DealDefinition> GetAllCachedDealDefinitions()
		{
			return base.GetCachedDeals();
		}
		
		public IEnumerable<DealSaleZoneGroup> GetDealSaleZoneGroupsByDealDefinitions(IEnumerable<DealDefinition> dealDefinitions)
		{
			if (dealDefinitions == null)
				return null;

			var result = new List<DealSaleZoneGroup>();

			foreach (DealDefinition dealDefinition in dealDefinitions)
			{
				DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Sale, true);
				dealDefinition.Settings.GetZoneGroups(context);
				if (context.SaleZoneGroups != null && context.SaleZoneGroups.Any())
				{
					foreach (var baseDealSaleZoneGroup in context.SaleZoneGroups)
					{
						var dealSaleZoneGroup = baseDealSaleZoneGroup as DealSaleZoneGroup;
						if (dealSaleZoneGroup == null)
							throw new Exception(String.Format("baseDealSaleZoneGroup should be of type 'Deal.Entities.DealSaleZoneGroup' and not of type '{0}'", baseDealSaleZoneGroup.GetType()));

						result.Add(dealSaleZoneGroup);
					}
				}
			}
			return result.Count > 0 ? result : null;
		}

		public IEnumerable<DealSupplierZoneGroup> GetDealSupplierZoneGroupsByDealDefinitions(IEnumerable<DealDefinition> dealDefinitions)
		{
			if (dealDefinitions == null)
				return null;

			var result = new List<DealSupplierZoneGroup>();

			foreach (DealDefinition dealDefinition in dealDefinitions)
			{
				DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Cost, true);
				dealDefinition.Settings.GetZoneGroups(context);
				if (context.SupplierZoneGroups != null && context.SupplierZoneGroups.Any())
				{
					foreach (var baseDealSupplierZoneGroups in context.SupplierZoneGroups)
					{
						var dealSupplierZoneGroup = baseDealSupplierZoneGroups as DealSupplierZoneGroup;
						if (dealSupplierZoneGroup == null)
							throw new Exception(String.Format("baseDealSupplierZoneGroups should be of type 'Deal.Entities.DealSupplierZoneGroup' and not of type '{0}'", baseDealSupplierZoneGroups.GetType()));

						result.Add(dealSupplierZoneGroup);
					}
				}
			}
			return result.Count > 0 ? result : null;
		}

		#endregion

		#region Private Methods
		DealZoneGroupTierDetails BuildDealZoneGroupTierDetails(DealSaleZoneGroupTierWithoutRate dealSaleZoneGroupTier, decimal rate, int currencyId)
		{
			return new DealZoneGroupTierDetails()
			{
				TierNb = dealSaleZoneGroupTier.TierNumber,
				VolumeInSeconds = dealSaleZoneGroupTier.VolumeInSeconds,
				CurrencyId = currencyId,
				Rate = rate,
				RetroActiveFromTierNb = dealSaleZoneGroupTier.RetroActiveFromTierNumber
			};
		}

		DealZoneGroupTierDetails BuildDealZoneGroupTierDetails(DealSupplierZoneGroupTierWithoutRate dealSupplierZoneGroupTier, decimal rate, int currencyId)
		{
			return new DealZoneGroupTierDetails()
			{
				TierNb = dealSupplierZoneGroupTier.TierNumber,
				VolumeInSeconds = dealSupplierZoneGroupTier.VolumeInSeconds,
				CurrencyId = currencyId,
				Rate = rate,
				RetroActiveFromTierNb = dealSupplierZoneGroupTier.RetroActiveFromTierNumber
			};
		}

		DealZoneGroupTierDetailsWithoutRate BuildDealZoneGroupTierDetailsWithoutRate(DealSaleZoneGroupTierWithoutRate dealSaleZoneGroupTier)
		{
			return new DealZoneGroupTierDetailsWithoutRate()
			{
				TierNb = dealSaleZoneGroupTier.TierNumber,
				VolumeInSeconds = dealSaleZoneGroupTier.VolumeInSeconds,
				RetroActiveFromTierNb = dealSaleZoneGroupTier.RetroActiveFromTierNumber
			};
		}

		DealZoneGroupTierDetailsWithoutRate BuildDealZoneGroupTierDetailsWithoutRate(DealSupplierZoneGroupTierWithoutRate dealSupplierZoneGroupTier)
		{
			return new DealZoneGroupTierDetailsWithoutRate()
			{
				TierNb = dealSupplierZoneGroupTier.TierNumber,
				VolumeInSeconds = dealSupplierZoneGroupTier.VolumeInSeconds,
				RetroActiveFromTierNb = dealSupplierZoneGroupTier.RetroActiveFromTierNumber
			};
		}

		DealSaleZoneGroupWithoutRate GetDealSaleZoneGroup(int dealId, int zoneGroupNb)
		{
			var cachedDealSaleZoneGroups = GetCachedDealSaleZoneGroupsWithoutRate();
			return cachedDealSaleZoneGroups.GetRecord(new DealZoneGroup() { DealId = dealId, ZoneGroupNb = zoneGroupNb });
		}

		DealSupplierZoneGroupWithoutRate GetDealSupplierZoneGroupWithoutRate(int dealId, int zoneGroupNb)
		{
			var cachedDealSupplierZoneGroups = GetCachedDealSupplierZoneGroupsWithoutRate();
			return cachedDealSupplierZoneGroups.GetRecord(new DealZoneGroup() { DealId = dealId, ZoneGroupNb = zoneGroupNb });
		}

		Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate> GetCachedDealSaleZoneGroupsWithoutRate()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDealSaleZoneGroups", () =>
			{
				Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate> result = new Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate>();
				var cachedDeals = base.GetCachedDeals();
				foreach (DealDefinition dealDefinition in cachedDeals.Values)
				{
					DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Sale, false);
					dealDefinition.Settings.GetZoneGroups(context);
					if (context.SaleZoneGroups != null)
					{
						foreach (var baseDealSaleZoneGroup in context.SaleZoneGroups)
						{
							var dealSaleZoneGroupWithoutRate = baseDealSaleZoneGroup as DealSaleZoneGroupWithoutRate;
							if (dealSaleZoneGroupWithoutRate == null)
								throw new Exception(String.Format("baseDealSaleZoneGroup should be of type 'Deal.Entities.DealSaleZoneGroupWithoutRate' and not of type '{0}'", baseDealSaleZoneGroup.GetType()));

							result.Add(new DealZoneGroup()
							{
								DealId = dealDefinition.DealId,
								ZoneGroupNb = dealSaleZoneGroupWithoutRate.DealSaleZoneGroupNb
							}
								, dealSaleZoneGroupWithoutRate);
						}
					}
				}
				return result;
			});
		}

		Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate> GetCachedDealSupplierZoneGroupsWithoutRate()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDealSupplierZoneGroups", () =>
			{
				Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate> result = new Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate>();
				var cachedDeals = base.GetCachedDeals();
				foreach (DealDefinition dealDefinition in cachedDeals.Values)
				{
					DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Cost, false);
					dealDefinition.Settings.GetZoneGroups(context);
					if (context.SupplierZoneGroups != null)
					{
						foreach (var baseDealSupplierZoneGroup in context.SupplierZoneGroups)
						{
							var dealSupplierZoneGroupWithoutRate = baseDealSupplierZoneGroup as DealSupplierZoneGroupWithoutRate;
							if (dealSupplierZoneGroupWithoutRate == null)
								throw new Exception(String.Format("baseDealSupplierZoneGroup should be of type 'Deal.Entities.DealSupplierZoneGroupWithoutRate'. and not of type '{0}'", baseDealSupplierZoneGroup.GetType()));

							result.Add(new DealZoneGroup()
							{
								DealId = dealDefinition.DealId,
								ZoneGroupNb = dealSupplierZoneGroupWithoutRate.DealSupplierZoneGroupNb
							}
								, dealSupplierZoneGroupWithoutRate);
						}
					}
				}
				return result;
			});
		}

		Dictionary<AccountZoneGroup, IOrderedEnumerable<DealSaleZoneGroupWithoutRate>> GetCachedAccountSaleZoneGroups()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountSaleZoneGroups", () =>
			{
				Dictionary<AccountZoneGroup, List<DealSaleZoneGroupWithoutRate>> result = new Dictionary<AccountZoneGroup, List<DealSaleZoneGroupWithoutRate>>();
				Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate> cachedDealZoneGroups = GetCachedDealSaleZoneGroupsWithoutRate();
				if (cachedDealZoneGroups != null)
				{
					foreach (var dealZoneGroupKvp in cachedDealZoneGroups)
					{
						DealSaleZoneGroupWithoutRate dealSaleZoneGroupWithoutRate = dealZoneGroupKvp.Value;
						if (dealSaleZoneGroupWithoutRate.Zones != null)
						{
							foreach (DealSaleZoneGroupZoneItem dealSaleZoneGroupZoneItem in dealSaleZoneGroupWithoutRate.Zones)
							{
								AccountZoneGroup accountZoneGroup = new AccountZoneGroup()
								{
									AccountId = dealSaleZoneGroupWithoutRate.CustomerId,
									ZoneId = dealSaleZoneGroupZoneItem.ZoneId
								};
								List<DealSaleZoneGroupWithoutRate> dealSaleZoneGroups = result.GetOrCreateItem(accountZoneGroup);
								dealSaleZoneGroups.Add(dealSaleZoneGroupWithoutRate);
							}
						}
					}
				}

				return result.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(item => item.BED));
			});
		}

		Dictionary<AccountZoneGroup, IOrderedEnumerable<DealSupplierZoneGroupWithoutRate>> GetCachedAccountSupplierZoneGroups()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountSupplierZoneGroups", () =>
			{
				Dictionary<AccountZoneGroup, List<DealSupplierZoneGroupWithoutRate>> result = new Dictionary<AccountZoneGroup, List<DealSupplierZoneGroupWithoutRate>>();
				Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate> cachedDealZoneGroups = GetCachedDealSupplierZoneGroupsWithoutRate();
				if (cachedDealZoneGroups != null)
				{
					foreach (var dealZoneGroupKvp in cachedDealZoneGroups)
					{
						DealSupplierZoneGroupWithoutRate dealSupplierZoneGroupWithoutRate = dealZoneGroupKvp.Value;
						if (dealSupplierZoneGroupWithoutRate.Zones != null)
						{
							foreach (DealSupplierZoneGroupZoneItem dealSupplierZoneGroupZoneItem in dealSupplierZoneGroupWithoutRate.Zones)
							{
								AccountZoneGroup accountZoneGroup = new AccountZoneGroup()
								{
									AccountId = dealSupplierZoneGroupWithoutRate.SupplierId,
									ZoneId = dealSupplierZoneGroupZoneItem.ZoneId
								};
								List<DealSupplierZoneGroupWithoutRate> dealSupplierZoneGroups = result.GetOrCreateItem(accountZoneGroup);
								dealSupplierZoneGroups.Add(dealSupplierZoneGroupWithoutRate);
							}
						}
					}
				}
				return result.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(item => item.BED));
			});
		}
		#endregion

		#region Mappers

		private DealDefinitionInfo DealDefinitionInfoMapper(DealDefinition dealDefinition)
		{
			return new DealDefinitionInfo()
			{
				DealId = dealDefinition.DealId,
				Name = dealDefinition.Name
			};
		}

		public override DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}