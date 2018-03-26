using System;
using System.Collections.Generic;
using Vanrise.Caching;
using Vanrise.Common;
using TOne.WhS.Deal.Data;
using TOne.WhS.Deal.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Deal.Business
{
	public class DealZoneRateManager
	{
		IDealZoneRateDataManager _dataManager;

		public DealZoneRateManager()
		{
			_dataManager = DealDataManagerFactory.GetDataManager<IDealZoneRateDataManager>();
		}

		#region public Methods

		public void InitializeDealZoneRateInsert(IEnumerable<int> dealIdsToKeep)
		{
			_dataManager.InitializeDealZoneRateInsert(dealIdsToKeep);
		}

		public void FinalizeDealZoneRateInsert()
		{
			_dataManager.FinalizeDealZoneRateInsert();
		}

		public DealZoneRate GetDealZoneRate(int dealId, int zoneGroupNb, long zoneId, int tierNb, bool isSale, DateTime effectiveTime)
		{
			var dealZoneGroupTierRates = this.GetDealZoneGroupZoneRates(dealId, zoneGroupNb, zoneId, isSale, effectiveTime);

			if (dealZoneGroupTierRates == null)
				return null;

			var dealZoneRates = dealZoneGroupTierRates.GetRecord(tierNb);

			if (dealZoneRates == null)
				return null;

			return dealZoneRates.FindRecord(item => item.IsEffective(effectiveTime));
		}

		public DealZoneRateByTireNB GetDealZoneGroupZoneRates(int dealId, int zoneGroupNb, long zoneId, bool isSale, DateTime effectiveTime)
		{
			var dealZoneGroupTierRates = this.GetDealZoneGroupRates(dealId, zoneGroupNb, isSale, effectiveTime);
			if (dealZoneGroupTierRates != null)
				return dealZoneGroupTierRates.GetRecord(zoneId);

			return null;
		}

		public DealZoneRateByZoneId GetDealZoneGroupRates(int dealId, int zoneGroupNb, bool isSale, DateTime effectiveTime)
		{
			var cachedEntities = this.GetCachedAllDealZoneRatesByDate(isSale, effectiveTime);

			var dealZoneGroup = new DealZoneGroup() { DealId = dealId, ZoneGroupNb = zoneGroupNb };

			return cachedEntities.GetRecord(dealZoneGroup);
		}

		public DealZoneRateByZoneGroup GetAllDealZoneRatesByDealIds(bool isSale, IEnumerable<int> dealIds)
		{
			var dealZoneRates = _dataManager.GetDealZoneRatesByDealIds(isSale, dealIds);
			return StructureDealZoneRatesByZoneGroup(dealZoneRates);
		}
		#endregion

		#region Private Methods

		private DealZoneRateByZoneGroup GetCachedAllDealZoneRatesByDate(bool isSale, DateTime effectiveDate)
		{
			GetCachedDealZoneRatesCacheName cacheObjectName = new GetCachedDealZoneRatesCacheName() { EffectiveOn = effectiveDate, IsSale = isSale };
			//var cacheObjectName = (isSale) ? "GetSaleDealZoneRates" : "GetPurchaseDealZoneRates";
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DealZoneRateCacheManager>().GetOrCreateObject(cacheObjectName, effectiveDate, () =>
			{
				var dealZoneRates = _dataManager.GetDealZoneRatesByDate(isSale, effectiveDate.Date, effectiveDate.Date.AddDays(1));
				return StructureDealZoneRatesByZoneGroup(dealZoneRates);
			});
		}

		private DealZoneRateByZoneGroup StructureDealZoneRatesByZoneGroup(IEnumerable<DealZoneRate> dealZoneRates)
		{
			var dealZoneRatesByZoneGroup = new DealZoneRateByZoneGroup();

			foreach (var dealZoneRate in dealZoneRates)
			{
				var dealZoneGroup = new DealZoneGroup() { DealId = dealZoneRate.DealId, ZoneGroupNb = dealZoneRate.ZoneGroupNb };
				var dealZoneRatesByZone = dealZoneRatesByZoneGroup.GetOrCreateItem(dealZoneGroup);

				var dealZoneRatesByTireNB = dealZoneRatesByZone.GetOrCreateItem(dealZoneRate.ZoneId);

				var dealZoneRateList = dealZoneRatesByTireNB.GetOrCreateItem(dealZoneRate.TierNb);

				dealZoneRateList.Add(dealZoneRate);
			}
			return dealZoneRatesByZoneGroup;

		}

		#endregion

		#region Private Classes

		private struct GetCachedDealZoneRatesCacheName : IBEDayFilterCacheName
		{
			public bool IsSale { get; set; }
			public DateTime EffectiveOn { get; set; }

			public DateTime FilterDay
			{
				get { return this.EffectiveOn; }
			}

			public override int GetHashCode()
			{
				return this.IsSale.GetHashCode() + this.EffectiveOn.GetHashCode();
			}
		}

		public class DealZoneRateCacheManager : BaseCacheManager<DateTime>
		{
			IDealZoneRateDataManager _dataManager = DealDataManagerFactory.GetDataManager<IDealZoneRateDataManager>();
			object _updateHandle;

			public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
			{
				get
				{
					return Vanrise.Caching.CacheObjectSize.ExtraLarge;
				}
			}

			public override T GetOrCreateObject<T>(object cacheName, DateTime parameter, Func<T> createObject)
			{
				return GetOrCreateObject(cacheName, parameter, BECacheExpirationChecker.Instance, createObject);
			}

			protected override bool ShouldSetCacheExpired(DateTime parameter)
			{
				return _dataManager.AreDealZoneRateUpdated(ref _updateHandle);
			}
		}
		#endregion

	}
}