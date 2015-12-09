using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierRateDataManager : IDataManager
    {
        BigResult<SupplierRate> GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<SupplierRateQuery> input);

        List<SupplierRate> GetSupplierRates(int supplierId, DateTime? minimumDate);

        List<SupplierRate> GetAllSupplierRates(DateTime? effectiveOn, bool isEffectiveInFuture);

        List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos);

        List<SupplierRate> GetEffectiveSupplierRates(int supplierId, DateTime effectiveDate);

        bool AreSupplierRatesUpdated(ref object updateHandle);
    }
}
