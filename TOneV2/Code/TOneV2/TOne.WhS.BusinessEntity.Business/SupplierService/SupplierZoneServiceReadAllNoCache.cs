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

        public SupplierDefaultService GetSupplierDefaultService(int supplierId)
        {
            if (_allSupplierDefaultServicesBySupplier == null)
                return null;

            return _allSupplierDefaultServicesBySupplier.GetRecord(supplierId);
        }

        public SupplierZoneServicesByZone GetSupplierZoneServicesByZone(int supplierId)
        {
            if (_allSupplierZoneServicesBySupplier == null)
                return null;

            return _allSupplierZoneServicesBySupplier.GetRecord(supplierId);
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
                SupplierPriceList supplierPriceList = supplierPriceListManager.GetPriceList(supplierDefaultService.PriceListId);
                result.Add(supplierPriceList.SupplierId, supplierDefaultService);
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

                if (!result.TryGetValue(supplierPriceList.SupplierId, out supplierZoneServicesByZone))
                {
                    supplierZoneServicesByZone = new SupplierZoneServicesByZone();
                    result.Add(supplierPriceList.SupplierId, supplierZoneServicesByZone);
                }

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
