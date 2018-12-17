﻿using System;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISupplierRateDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
        object _updateHandle;

        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }

        public override T GetOrCreateObject<T>(object cacheName, Func<T> createObject)
        {
            return GetOrCreateObject(cacheName, BECacheExpirationChecker.Instance, createObject);
        }


        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSupplierRatesUpdated(ref _updateHandle);
        }

        public SupplierRate CacheAndGetRate(SupplierRate rate)
        {
            return rate;
        }
    }
}
