using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateReadAllNoCache : ISupplierRateReader
    {
        #region ctor/Local Variables
        private DateTime? _effectiveOn { get; set; }
        private bool _isEffectiveInFuture { get; set; }

        private Dictionary<int, SupplierRatesByZone> _supplierRatesByZones { get; set; }
        #endregion


        public SupplierRateReadAllNoCache(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _effectiveOn = effectiveOn;
            _isEffectiveInFuture = isEffectiveInFuture;
            _supplierRatesByZones = GetRatesBySuppliers(supplierInfos);
        }

        public SupplierRatesByZone GetSupplierRates(int supplierId)
        {
            if (_supplierRatesByZones == null)
                return null;
            return _supplierRatesByZones.GetRecord(supplierId);
        }

        private Dictionary<int, SupplierRatesByZone> GetRatesBySuppliers(IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            Dictionary<int, SupplierRatesByZone> result = new Dictionary<int, SupplierRatesByZone>();
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();

            List<SupplierRate> supplierRates = dataManager.GetEffectiveSupplierRatesBySuppliers(_effectiveOn, _isEffectiveInFuture, supplierInfos);
            SupplierRatesByZone supplierRatesByZone;
            SupplierZoneRate supplierZoneRate;

            SupplierPriceListManager supplierPriceListManager = new SupplierPriceListManager();

            foreach (SupplierRate supplierRate in supplierRates)
            {
                SupplierPriceList supplierPriceList = supplierPriceListManager.GetPriceList(supplierRate.PriceListId);

                if (!result.TryGetValue(supplierPriceList.SupplierId, out supplierRatesByZone))
                {
                    supplierRatesByZone = new SupplierRatesByZone();
                    result.Add(supplierPriceList.SupplierId, supplierRatesByZone);
                }

                if (!supplierRatesByZone.TryGetValue(supplierRate.ZoneId, out supplierZoneRate))
                {
                    supplierZoneRate = new SupplierZoneRate();
                    supplierRatesByZone.Add(supplierRate.ZoneId, supplierZoneRate);
                }

                if (supplierRate.RateTypeId.HasValue)
                {
                    if (supplierZoneRate.RatesByRateType == null)
                        supplierZoneRate.RatesByRateType = new Dictionary<int, SupplierRate>();
                    supplierZoneRate.RatesByRateType.Add(supplierRate.RateTypeId.Value, supplierRate);
                }
                else
                    supplierZoneRate.Rate = supplierRate;
            }
            return result;
        }
    }
}