using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierRateDataManager : IDataManager
    {

        IEnumerable<SupplierRate> GetFilteredSupplierRates(SupplierRateQuery input);

        List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate);

        List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos);

        List<SupplierRate> GetSupplierRatesInBetweenPeriod(DateTime froDateTime, DateTime tillDateTime);
        List<SupplierRate> GetEffectiveSupplierRates(DateTime effectiveDate);

        bool AreSupplierRatesUpdated(ref object updateHandle);

        SupplierRate GetSupplierRateById(long rateId);
    }
}
