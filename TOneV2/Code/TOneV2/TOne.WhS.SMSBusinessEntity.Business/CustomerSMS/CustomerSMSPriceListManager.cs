using System;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSPriceListManager
    {
        public CustomerSMSPriceList CreateCustomerSMSPriceList(int customerID, int currencyID, DateTime effectiveDate, long processInstanceID, int userID)
        {
            return new CustomerSMSPriceList()
            {
                ID = ReserveIdRange(1),
                CustomerID = customerID,
                CurrencyID = currencyID,
                EffectiveOn = effectiveDate,
                UserID = userID,
                ProcessInstanceID = processInstanceID
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