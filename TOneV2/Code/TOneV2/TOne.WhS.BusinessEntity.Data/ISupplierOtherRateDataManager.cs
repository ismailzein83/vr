using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierOtherRateDataManager : IDataManager
    {

        IEnumerable<SupplierOtherRate> GetFilteredSupplierOtherRates(SupplierOtherRateQuery input);

        IEnumerable<SupplierOtherRate> GetSupplierOtherRates(IEnumerable<long> zoneIds, DateTime effectiveOn);
    }
}
