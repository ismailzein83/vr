using System;
using System.Collections.Generic;
using TOne.WhS.SMSBusinessEntity.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ICustomerSMSRateDataManager : IDataManager
    {
        List<CustomerSMSRate> GetCustomerSMSRatesEffectiveAfter(int customerID, DateTime effectiveDate);

        List<CustomerSMSRate> GetCustomerSMSRatesEffectiveOn(DateTime effectiveDate);

        bool ApplySaleRates(CustomerSMSPriceList customerSMSPriceList, Dictionary<int, CustomerSMSRateChange> saleRateChangesByMobileNetworkId);

        bool AreCustomerSMSRatesUpdated(ref object updateHandle);
    }
}