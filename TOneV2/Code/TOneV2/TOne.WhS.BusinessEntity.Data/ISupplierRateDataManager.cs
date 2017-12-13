using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierRateDataManager : IDataManager
    {
        IEnumerable<SupplierRate> GetZoneRateHistory(List<long> zoneIds, List<int> countryIds, int supplierId);
        IEnumerable<SupplierRate> GetFilteredSupplierRates(SupplierRateQuery input, DateTime effectiveOn);
        IEnumerable<SupplierRate> GetFilteredSupplierPendingRates(SupplierRateQuery input, DateTime effectiveOn);
        IEnumerable<SupplierRate> GetSupplierRatesForZone(SupplierRateForZoneQuery input, DateTime effectiveOn);
        List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate);
        List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos);
        List<SupplierRate> GetSupplierRatesInBetweenPeriod(DateTime fromDateTime, DateTime tillDateTime);
        List<SupplierRate> GetEffectiveSupplierRates(DateTime fromDate, DateTime toDate);
        bool AreSupplierRatesUpdated(ref object updateHandle);
        SupplierRate GetSupplierRateById(long rateId);
        List<SupplierRate> GetSupplierRates(HashSet<long> supplierRateIds);
        DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate);
        object GetMaximumTimeStamp();
    }
}