using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSPriceListManager
    {
        ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();

        #region Public Methods
        //public Vanrise.Entities.IDataRetrievalResult<CustomerSMSPriceListDetail> GetFilteredCustomerSMSPriceLists(DataRetrievalInput<CustomerSMSPriceListQuery> input)
        //{
        //    return BigDataManager.Instance.RetrieveData(input, new CustomerSMSPriceListRequestHandler());
        //}

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

        public CustomerSMSPriceList GetCustomerSMSPriceListByID(long priceListID)
        {
            var customerSMSPriceLists = GetCachedCustomerSMSPriceLists();
            return customerSMSPriceLists != null ? customerSMSPriceLists.GetRecord(priceListID): null;
        }

        public Dictionary<long, CustomerSMSPriceList> GetCachedCustomerSMSPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustomerSMSPriceLists", () =>
            {
                var priceLists = _customerSMSPriceListDataManager.GetCustomerSMSPriceLists();
                return priceLists != null ? priceLists.ToDictionary(item => item.ID, item => item) : null;
            });
        }

        #endregion

        #region Private Methods

        private long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIds, out startingId);
            return startingId;
        }

        #endregion


        #region Internal/Private Classes
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _customerSMSPriceListDataManager.AreCustomerSMSPriceListUpdated(ref _updateHandle);
            }
        }

        //private class CustomerSMSPriceListRequestHandler : BigDataRequestHandler<CustomerSMSPriceListQuery, CustomerSMSPriceList, CustomerSMSPriceListDetail>
        //{
        //    ICustomerSMSPriceListDataManager _customerSMSPriceListDataManager = SMSBEDataFactory.GetDataManager<ICustomerSMSPriceListDataManager>();
        //    public override CustomerSMSPriceListDetail EntityDetailMapper(CustomerSMSPriceList entity)
        //    {
        //        return new CustomerSMSPriceListDetail()
        //        {
        //            ID = entity.ID,
        //            CustomerID = entity.CustomerID,
        //            CurrencyID = entity.CurrencyID,
        //            CustomerName = new CarrierAccountManager().GetCarrierAccountName(entity.CustomerID),
        //            EffectiveOn = entity.EffectiveOn,
        //            ProcessInstanceID = entity.ProcessInstanceID,
        //            UserID = entity.UserID
        //        };
        //    }

        //    public override IEnumerable<CustomerSMSPriceList> RetrieveAllData(DataRetrievalInput<CustomerSMSPriceListQuery> input)
        //    {
        //        return _customerSMSPriceListDataManager.GetCustomerSMSPriceLists();
        //    }
        //}

        #endregion
    }
}