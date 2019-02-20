using System;
using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ISupplierSMSRateDataManager : IDataManager
    {
        List<SupplierSMSRate> GetSupplierSMSRatesEffectiveAfter(int supplierID, DateTime effectiveDate);

        List<SupplierSMSRate> GetSupplierSMSRatesEffectiveOn(DateTime fromDate, DateTime toDate);

        List<SupplierSMSRate> GetSupplierSMSRatesEffectiveOn(DateTime effectiveOn);

        bool ApplySupplierRates(SupplierSMSPriceList supplierSMSPriceList, Dictionary<int, SupplierSMSRateChange> supplierRateChangesByMobileNetworkId);

        bool AreSupplierSMSRatesUpdated(ref object updateHandle);
    }
}