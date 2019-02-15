using System;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class SupplierSMSPriceListManager
    {
        public SupplierSMSPriceList CreateSupplierSMSPriceList(int supplierID, int currencyID, DateTime effectiveDate, long processInstanceID, int userID)
        {
            return new SupplierSMSPriceList()
            {
                ID = ReserveIdRange(1),
                SupplierID = supplierID,
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