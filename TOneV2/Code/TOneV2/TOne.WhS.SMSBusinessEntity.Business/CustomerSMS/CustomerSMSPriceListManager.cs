using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSPriceListManager
    {
        ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();

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

        public Dictionary<long, CustomerSMSPriceList> GetCachedCustomerSMSPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustomerSMSPriceLists", () =>
            {
                var priceLists = _customerSMSPriceListDataManager.GetCustomerSMSPriceLists();
                return priceLists != null ? priceLists.ToDictionary(item => item.ID, item => item) : null;
            });
        }

        private long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
            return startingId;
        }

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _customerSMSPriceListDataManager.AreCustomerSMSPriceListUpdated(ref _updateHandle);
            }
        }
    }
}