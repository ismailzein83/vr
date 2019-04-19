﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

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
        public IEnumerable<SupplierOtherRate> GetSupplierOtherRates(IEnumerable<long> zoneIds, DateTime effectiveOn)
        {
            SupplierRateDataManager supplierRateDataManager = new SupplierRateDataManager();
            return supplierRateDataManager.GetSupplierOtherRates(zoneIds, effectiveOn);
        }
        #endregion

    }
}
