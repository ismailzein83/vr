using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SMSBusinessEntity.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface ICustomerSMSRateDataManager : IDataManager
    {
        List<CustomerSMSRate> GetCustomerSMSRatesEffectiveAfter(int customerID, DateTime effectiveDate);
        bool ApplySaleRates(CustomerSMSPriceList customerSMSPriceList, Dictionary<int, CustomerSMSRateChange> saleRateChangesByMobileNetworkId);
    }
}