﻿using System;
using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ISupplierSMSRateDataManager : IDataManager
    {
        List<SupplierSMSRate> GetSupplierSMSRatesEffectiveAfter(int supplierID, DateTime effectiveDate);
        bool ApplySupplierRates(SupplierSMSPriceList supplierSMSPriceList, Dictionary<int, SupplierSMSRateChange> supplierRateChangesByMobileNetworkId);
    }
}