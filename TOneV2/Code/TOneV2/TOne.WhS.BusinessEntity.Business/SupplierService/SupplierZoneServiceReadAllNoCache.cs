using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceReadAllNoCache : ISupplierZoneServiceReader
    {
        #region ctor/Local Variables

        Dictionary<int, SupplierZoneServicesByZone> _allSupplierZoneServicesBySupplier;
        Dictionary<int, SupplierDefaultService> _allSupplierDefaultServicesBySupplier;

        #endregion


        #region Public Methods
        public SupplierZoneServiceReadAllNoCache(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _allSupplierDefaultServicesBySupplier = GetAllSupplierDefaultServicesBySupplier(supplierInfos, effectiveOn, isEffectiveInFuture);
            _allSupplierZoneServicesBySupplier = GetAllSupplierZoneServicesBySupplier(supplierInfos, effectiveOn, isEffectiveInFuture);
        }

        public SupplierDefaultService GetSupplierDefaultService(int supplierId, DateTime effectiveOn)
        {
            if (_allSupplierDefaultServicesBySupplier == null)
                return null;

            return _allSupplierDefaultServicesBySupplier.GetRecord(supplierId);
        }

        public SupplierZoneService GetSupplierZoneServicesByZone(int supplierId, long supplierZoneId, DateTime effectiveOn)
        {
            if (_allSupplierZoneServicesBySupplier == null)
                return null;

            SupplierZoneServicesByZone supplierZoneServicesByZone = _allSupplierZoneServicesBySupplier.GetRecord(supplierId);
            if (supplierZoneServicesByZone == null)
                return null;

            return supplierZoneServicesByZone.GetRecord(supplierZoneId);

        }

        #endregion

        #region Private Methods

        private Dictionary<int, SupplierDefaultService> GetAllSupplierDefaultServicesBySupplier(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            Dictionary<int, SupplierDefaultService> result = new Dictionary<int, SupplierDefaultService>();

            ISupplierZoneServiceDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            List<SupplierDefaultService> supplierDefaultServices = _dataManager.GetEffectiveSupplierDefaultServicesBySuppliers(supplierInfos, effectiveOn, isEffectiveInFuture);

            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();
            foreach (SupplierDefaultService supplierDefaultService in supplierDefaultServices)
            {
                result.Add(supplierDefaultService.SupplierId.Value, supplierDefaultService);
            }

            return result;
        }

        private Dictionary<int, SupplierZoneServicesByZone> GetAllSupplierZoneServicesBySupplier(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            Dictionary<int, SupplierZoneServicesByZone> result = new Dictionary<int, SupplierZoneServicesByZone>();
            ISupplierZoneServiceDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();

            List<SupplierZoneService> supplierZoneServices = _dataManager.GetEffectiveSupplierZoneServicesBySuppliers(supplierInfos, effectiveOn, isEffectiveInFuture);
            SupplierZoneServicesByZone supplierZoneServicesByZone;
            SupplierZoneService tempSupplierZoneService;

            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();

            foreach (SupplierZoneService supplierZoneService in supplierZoneServices)
            {
                SupplierPriceList supplierPriceList = supplierPriceListManager.GetPriceList(supplierZoneService.PriceListId);
                supplierZoneServicesByZone = result.GetOrCreateItem(supplierPriceList.SupplierId);

                if (!supplierZoneServicesByZone.TryGetValue(supplierZoneService.ZoneId, out tempSupplierZoneService))
                {
                    supplierZoneServicesByZone.Add(supplierZoneService.ZoneId, supplierZoneService);
                }
            }

            return result;
        }

        #endregion
    }
}