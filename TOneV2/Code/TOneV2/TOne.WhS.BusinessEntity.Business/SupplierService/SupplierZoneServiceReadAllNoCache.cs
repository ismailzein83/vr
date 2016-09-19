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
        #endregion


        #region Public Methods
        public SupplierZoneServiceReadAllNoCache(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _allSupplierZoneServicesBySupplier = GetAllSupplierZoneServicesBySupplier(supplierInfos, effectiveOn, isEffectiveInFuture);
        }

        public SupplierZoneServicesByZone GetSupplierZoneServicesByZone(int supplierId)
        {
            if (_allSupplierZoneServicesBySupplier == null)
                return null;

            return _allSupplierZoneServicesBySupplier.GetRecord(supplierId);
        }
        #endregion


        #region Private Methods
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
