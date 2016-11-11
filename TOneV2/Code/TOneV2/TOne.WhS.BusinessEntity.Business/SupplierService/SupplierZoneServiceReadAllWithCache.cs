using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceReadAllWithCache : ISupplierZoneServiceReader
    {
        #region ctor/Local Variables

        private DateTime _effectiveOn;

        #endregion


        #region Public Methods

        public SupplierZoneServiceReadAllWithCache(DateTime effectiveOn)
        {
            _effectiveOn = effectiveOn;
        }

        public SupplierDefaultService GetSupplierDefaultService(int supplierId, DateTime effectiveOn)
        {
            Dictionary<int, List<SupplierDefaultService>> defaultZoneServicesBySupplier = GetCachedSupplierDefaultServices();
            List<SupplierDefaultService> supplierDefaultServices =  defaultZoneServicesBySupplier.GetRecord(supplierId);

            if (supplierDefaultServices == null)
                return null;

            return Helper.GetBusinessEntityInfo<SupplierDefaultService>(supplierDefaultServices, effectiveOn);
        }

        public SupplierZoneService GetSupplierZoneServicesByZone(int supplierId, long supplierZoneId, DateTime effectiveOn)
        {
            SupplierZoneServicesByZoneData supplierZoneServicesByZoneData = GetCachedSupplierZoneServices(supplierId);
            if (supplierZoneServicesByZoneData == null)
                return null;

            var supplierZoneServices = supplierZoneServicesByZoneData.GetRecord(supplierZoneId);
            if (supplierZoneServices == null)
                return null;

            return Helper.GetBusinessEntityInfo<SupplierZoneService>(supplierZoneServices, effectiveOn);
        }


        #endregion


        #region Private Methods

        private struct GetCachedSupplierDefaultServicesCacheName
        {
            public DateTime EffectiveOn { get; set; }
        }

        private Dictionary<int, List<SupplierDefaultService>> GetCachedSupplierDefaultServices()
        {
            DateTimeRange dateTimeRange = Helper.GetDateTimeRangeWithOffset(_effectiveOn);

            var cacheName = new GetCachedSupplierDefaultServicesCacheName { EffectiveOn = _effectiveOn.Date };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierZoneServiceCacheManager>().GetOrCreateObject(cacheName, () =>
            {
                Dictionary<int, List<SupplierDefaultService>> supplierZoneServicesBySupplier = new Dictionary<int, List<SupplierDefaultService>>();

                ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
                IEnumerable<SupplierDefaultService> defaultServices = dataManager.GetEffectiveSupplierDefaultServices(dateTimeRange.From, dateTimeRange.To);

                if (defaultServices != null)
                {
                    foreach (SupplierDefaultService defaultService in defaultServices)
                    {
                        List<SupplierDefaultService> supplierDefaultServices = supplierZoneServicesBySupplier.GetOrCreateItem(defaultService.SupplierId);
                        supplierDefaultServices.Add(defaultService);
                    }
                }
                return supplierZoneServicesBySupplier;
            });
        }

        private struct GetCachedSupplierZoneServicesCacheName
        {
            public int SupplierId { get; set; }

            public DateTime EffectiveOn { get; set; }
        }

        private SupplierZoneServicesByZoneData GetCachedSupplierZoneServices(int supplierId)
        {
            DateTimeRange dateTimeRange = Helper.GetDateTimeRangeWithOffset(_effectiveOn);

            var cacheName = new GetCachedSupplierZoneServicesCacheName { SupplierId = supplierId, EffectiveOn = _effectiveOn.Date };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierZoneServiceCacheManager>().GetOrCreateObject(cacheName, () =>
            {
                var zoneServicesByZone = new SupplierZoneServicesByZoneData();

                ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
                IEnumerable<SupplierZoneService> zoneServices = dataManager.GetEffectiveSupplierZoneServices(supplierId, dateTimeRange.From, dateTimeRange.To);

                if (zoneServices != null)
                {
                    foreach (SupplierZoneService zoneService in zoneServices)
                    {
                        var supplierZoneServices = zoneServicesByZone.GetOrCreateItem(zoneService.ZoneId);
                        supplierZoneServices.Add(zoneService);
                    }
                }

                return zoneServicesByZone;
            });
        }

        #endregion
    }
}
