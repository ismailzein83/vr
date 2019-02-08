using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSPriceListManager
    {
        public CustomerSMSPriceList PrepareCustomerSMSPriceListData(int customerID, DateTime effectiveDate)
        {
            Currency currency = new BusinessEntity.Business.CurrencyManager().GetCurrencyByCarrierId(customerID.ToString());
            long priceListID = ReserveIdRange(1);
            return new CustomerSMSPriceList()
            {
                 ID = priceListID,
                CustomerID = customerID,
                CurrencyID = currency.CurrencyID,
                EffectiveOn = effectiveDate,
                UserID = SecurityContext.Current.GetLoggedInUserId(),
                ProcessInstanceID = null,
            };
        }

        private long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
            return startingId;
        }
    }
}
