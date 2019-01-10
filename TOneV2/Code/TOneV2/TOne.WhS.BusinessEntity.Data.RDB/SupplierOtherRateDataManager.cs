using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierOtherRateDataManager : ISupplierOtherRateDataManager
    {
        #region ISupplierOtherRateDataManager Members

        public IEnumerable<SupplierOtherRate> GetFilteredSupplierOtherRates(SupplierOtherRateQuery input)
        {
            SupplierRateDataManager supplierRateDataManager = new SupplierRateDataManager();
            return supplierRateDataManager.GetFilteredSupplierOtherRates(input.ZoneId, input.EffectiveOn);
        }

        #endregion

    }
}
