using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierRateDataManager : IDataManager
    {
        IEnumerable<SupplierRate> GetZoneRateHistory(List<long> zoneIds, List<int> countryIds, int supplierId);
        IEnumerable<SupplierRate> GetFilteredSupplierRates(SupplierRateQuery input);
        IEnumerable<SupplierRate> GetFilteredSupplierPendingRates(SupplierRateQuery input);

        List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate);

        List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos);

        List<SupplierRate> GetSupplierRatesInBetweenPeriod(DateTime fromDateTime, DateTime tillDateTime);
        List<SupplierRate> GetEffectiveSupplierRates(DateTime fromDate, DateTime toDate);

        bool AreSupplierRatesUpdated(ref object updateHandle);

        SupplierRate GetSupplierRateById(long rateId);
    }
}
