using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

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

        public SupplierDefaultService GetSupplierDefaultService(int supplierId)
        {
            Dictionary<int, SupplierDefaultService> defaultZoneServicesBySupplier = GetCachedSupplierDefaultServices();
            return defaultZoneServicesBySupplier.GetRecord(supplierId);
        }

        public SupplierZoneServicesByZone GetSupplierZoneServicesByZone(int supplierId)
        {
            return GetCachedSupplierZoneServices(supplierId);
        }

      
        #endregion


        #region Private Methods

        private struct GetCachedSupplierDefaultServicesCacheName
        {
            public DateTime EffectiveOn { get; set; }
        }

        private Dictionary<int, SupplierDefaultService> GetCachedSupplierDefaultServices()
        {
            var cacheName = new GetCachedSupplierDefaultServicesCacheName { EffectiveOn = _effectiveOn.Date };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierZoneServiceCacheManager>().GetOrCreateObject(cacheName, () =>
            {
                Dictionary<int, SupplierDefaultService> supplierZoneServicesBySupplier = new Dictionary<int, SupplierDefaultService>();

                ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
                IEnumerable<SupplierDefaultService> defaultServices = dataManager.GetEffectiveSupplierDefaultServices(_effectiveOn);

                if (defaultServices != null)
                {
                    SupplierDefaultService supplierDefaultService;

                    foreach (SupplierDefaultService defaultService in defaultServices)
                    {
                        if (!supplierZoneServicesBySupplier.TryGetValue(defaultService.SupplierId.Value, out supplierDefaultService))
                        {
                            supplierDefaultService = new SupplierDefaultService()
                            {
                                SupplierId = defaultService.SupplierId.Value,
                                BED = defaultService.BED,
                                EffectiveServices = defaultService.EffectiveServices,
                                ReceivedServices = defaultService.ReceivedServices
                            };

                            supplierZoneServicesBySupplier.Add(defaultService.SupplierId.Value, supplierDefaultService);
                        }
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

        private SupplierZoneServicesByZone GetCachedSupplierZoneServices(int supplierId)
        {
            var cacheName = new GetCachedSupplierZoneServicesCacheName { SupplierId = supplierId, EffectiveOn = _effectiveOn.Date };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierZoneServiceCacheManager>().GetOrCreateObject(cacheName, () =>
            {
                var zoneServicesByZone = new SupplierZoneServicesByZone();

                ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
                IEnumerable<SupplierZoneService> zoneServices = dataManager.GetEffectiveSupplierZoneServices(supplierId, _effectiveOn);

                if (zoneServices != null)
                {
                    foreach (SupplierZoneService zoneService in zoneServices)
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
