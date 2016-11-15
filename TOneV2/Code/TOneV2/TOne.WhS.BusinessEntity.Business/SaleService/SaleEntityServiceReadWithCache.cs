﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceReadWithCache : ISaleEntityServiceReader
    {
        #region Variables/Ctor

        DateTime _effectiveOn;

        public SaleEntityServiceReadWithCache(DateTime effectiveOn)
        {
            _effectiveOn = effectiveOn;
        }

        #endregion

        #region Private Methods

        public SaleEntityDefaultService GetSaleEntityDefaultService(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            SaleEntityDefaultServicesByOwner defaultServicesByOwner = GetCachedSaleEntityDefaultServicesByOwner();
            SaleEntityDefaultService defaultService = null;

            Dictionary<int, SaleEntityDefaultService> defaultServicesByTargetOwner = ownerType == SalePriceListOwnerType.SellingProduct ?
                defaultServicesByOwner.DefaultServicesByProduct :
                defaultServicesByOwner.DefaultServicesByCustomer;

            defaultServicesByTargetOwner.TryGetValue(ownerId, out defaultService);
            return defaultService;
        }
        public SaleEntityZoneServicesByZone GetSaleEntityZoneServicesByZone(BusinessEntity.Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            return GetCachedSaleEntityZoneServicesByZone(ownerType, ownerId);
        }

        #endregion

        #region Private Methods

        private struct GetCachedSaleEntityDefaultServicesCacheName : IBEDayFilterCacheName
        {
            public DateTime EffectiveOn { get; set; }

            public DateTime FilterDay
            {
                get { return this.EffectiveOn; }
            }
        }

        private SaleEntityDefaultServicesByOwner GetCachedSaleEntityDefaultServicesByOwner()
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityServiceCacheManager>();
            var cacheName = new GetCachedSaleEntityDefaultServicesCacheName
            {
                EffectiveOn = _effectiveOn.Date
            };

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityServiceCacheManager>().GetOrCreateObject(cacheName, () =>
            {
                var defaultServicesByOwner = new SaleEntityDefaultServicesByOwner();
                defaultServicesByOwner.DefaultServicesByProduct = new Dictionary<int, SaleEntityDefaultService>();
                defaultServicesByOwner.DefaultServicesByCustomer = new Dictionary<int, SaleEntityDefaultService>();

                var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
                IEnumerable<SaleEntityDefaultService> defaultServices = dataManager.GetEffectiveSaleEntityDefaultServices(_effectiveOn);

                if (defaultServices != null)
                {
                    var salePriceListManager = new SalePriceListManager();

                    foreach (SaleEntityDefaultService defaultService in defaultServices)
                    {
                        if (salePriceListManager.IsSalePriceListDeleted(defaultService.PriceListId))
                            continue;

                        SalePriceList priceList = salePriceListManager.GetPriceList(defaultService.PriceListId);

                        if (priceList.OwnerType == SalePriceListOwnerType.SellingProduct)
                        {
                            if (!defaultServicesByOwner.DefaultServicesByProduct.ContainsKey(priceList.OwnerId))
                                defaultServicesByOwner.DefaultServicesByProduct.Add(priceList.OwnerId, defaultService);
                        }
                        else
                        {
                            if (!defaultServicesByOwner.DefaultServicesByCustomer.ContainsKey(priceList.OwnerId))
                                defaultServicesByOwner.DefaultServicesByCustomer.Add(priceList.OwnerId, defaultService);
                        }
                    }
                }

                return defaultServicesByOwner;
            });
        }

        private struct GetCachedSaleEntityZoneServicesCacheName : IBEDayFilterCacheName
        {
            public DateTime EffectiveOn { get; set; }

            public SalePriceListOwnerType OwnerType { get; set; }

            public int OwnerId { get; set; }

            public DateTime FilterDay
            {
                get { return this.EffectiveOn; }
            }
        }

        private SaleEntityZoneServicesByZone GetCachedSaleEntityZoneServicesByZone(SalePriceListOwnerType ownerType, int ownerId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityServiceCacheManager>();
            var cacheName = new GetCachedSaleEntityZoneServicesCacheName
            {
                EffectiveOn = _effectiveOn.Date,
                OwnerType = ownerType,
                OwnerId = ownerId
            };

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleEntityServiceCacheManager>().GetOrCreateObject(cacheName, () =>
            {
                var zoneServicesByZone = new SaleEntityZoneServicesByZone();

                var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
                IEnumerable<SaleEntityZoneService> zoneServices = dataManager.GetEffectiveSaleEntityZoneServices(ownerType, ownerId, _effectiveOn);

                if (zoneServices != null)
                {
                    foreach (SaleEntityZoneService zoneService in zoneServices)
                    {
                        if (!zoneServicesByZone.ContainsKey(zoneService.ZoneId))
                            zoneServicesByZone.Add(zoneService.ZoneId, zoneService);
                    }
                }

                return zoneServicesByZone;
            });
        }

        #endregion
    }
}