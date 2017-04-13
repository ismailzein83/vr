using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePriceListChangeManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SalePricelistRateChange> GetFilteredPricelistRateChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges = dataManager.GetFilteredSalePricelistRateChanges(input.Query.PriceListId, input.Query.Countries);
            return DataRetrievalManager.Instance.ProcessResult(input, salePriceListRateChanges.ToBigResult(input, null, SalePricelistRateChangeDetailMapper));
        }

        public Vanrise.Entities.IDataRetrievalResult<SalePricelistCodeChange> GetFilteredPricelistCodeChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges = dataManager.GetFilteredSalePricelistCodeChanges(input.Query.PriceListId, input.Query.Countries);
            return DataRetrievalManager.Instance.ProcessResult(input, salePriceListRateChanges.ToBigResult(input, null, SalePricelistCodeChangeDetailMapper));
        }

        public string GetOwnerName(int priceListId)
        {
            string ownerName = string.Empty;
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var priceList = salePriceListManager.GetPriceList(priceListId);
            if (priceList != null)
            {
                var ownerId = priceList.OwnerId;
                ownerName = carrierAccountManager.GetCarrierAccountName(ownerId);
            }
            return ownerName;
        }
        public void SaveSalePriceListCustomerChanges(List<CustomerPriceListChange> customerPriceListChanges, long processInstanceId)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            List<SalePriceListCustomerChange> todbCustomerChanges = new List<SalePriceListCustomerChange>();
            List<SalePricelistCodeChange> todbCodeChanges = new List<SalePricelistCodeChange>();
            List<SalePricelistRateChange> todbsaleRateChanges = new List<SalePricelistRateChange>();
            Dictionary<string, SalePricelistCodeChange> salePricelistCodeChanges = new Dictionary<string, SalePricelistCodeChange>();

            foreach (var priceListChange in customerPriceListChanges)
            {
                Dictionary<string, SalePriceListCustomerChange> salePriceListCustomerChanges = new Dictionary<string, SalePriceListCustomerChange>();

                foreach (var codeChange in priceListChange.CodeChanges)
                {
                    string key = string.Format("{0}|{1}", codeChange.CountryId, priceListChange.PriceListId);
                    SalePriceListCustomerChange customer;
                    if (!salePriceListCustomerChanges.TryGetValue(key, out customer))
                    {
                        customer = new SalePriceListCustomerChange
                        {
                            CountryId = codeChange.CountryId,
                            PriceListId = priceListChange.PriceListId,
                            CustomerId = priceListChange.CustomerId,
                            BatchId = processInstanceId
                        };
                        salePriceListCustomerChanges.Add(key, customer);
                        todbCustomerChanges.Add(customer);
                    }
                    string key1 = string.Format("{0}|{1}", codeChange.CountryId, codeChange.Code);
                    if (!salePricelistCodeChanges.ContainsKey(key1))
                    {
                        SalePricelistCodeChange salePricelistCodeChange = new SalePricelistCodeChange
                        {
                            CountryId = codeChange.CountryId,
                            PricelistId = customer.PriceListId,
                            ZoneName = codeChange.ZoneName,
                            RecentZoneName = codeChange.RecentZoneName,
                            EED = codeChange.EED,
                            BED = codeChange.BED,
                            Code = codeChange.Code,
                            ChangeType = codeChange.ChangeType,
                            BatchId = processInstanceId
                        };
                        salePricelistCodeChanges.Add(key1, salePricelistCodeChange);
                        todbCodeChanges.Add(salePricelistCodeChange);
                    }
                }
                foreach (var rate in priceListChange.RateChanges)
                {
                    SalePricelistRateChange rateChange = new SalePricelistRateChange
                    {
                        CountryId = rate.CountryId,
                        ZoneName = rate.ZoneName,
                        Rate = rate.Rate,
                        PricelistId = priceListChange.PriceListId,
                        ChangeType = rate.ChangeType,
                        BED = rate.BED,
                        RecentRate = rate.RecentRate,
                        EED = rate.EED
                    };
                    todbsaleRateChanges.Add(rateChange);
                }
            }
            dataManager.SaveCustomerChangesToDb(todbCustomerChanges);
            dataManager.SaveCustomerCodeChangesToDb(todbCodeChanges);
            dataManager.SaveCustomerRateChangesToDb(todbsaleRateChanges, processInstanceId);
        }

        public CustomerPriceListChange GetCustomerChangesByPriceListId(int pricelistId)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var salePriceListRateChanges = dataManager.GetFilteredSalePricelistRateChanges(pricelistId, null);
            var salePriceListCodeChanges = dataManager.GetFilteredSalePricelistCodeChanges(pricelistId, null);
            CustomerPriceListChange changes = new CustomerPriceListChange();
            changes.CodeChanges.AddRange(salePriceListCodeChanges);
            changes.RateChanges.AddRange(salePriceListRateChanges);
            changes.PriceListId = pricelistId;

            return changes;
        }
        public Dictionary<int, List<CustomerPriceListChange>> GetNotSentChangesByCustomer(IEnumerable<int> customerIds)
        {
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            List<SalePricelistCodeChange> codeChanges = dataManager.GetNotSentCodechanges(customerIds);
            List<SalePricelistRateChange> rateChanges = dataManager.GetNotSentRatechanges(customerIds);
            var customerPriceListChanges = new Dictionary<int, CustomerPriceListChange>();
            foreach (var codeChange in codeChanges)
            {
                CustomerPriceListChange customerPriceList;
                if (!customerPriceListChanges.TryGetValue(codeChange.PricelistId, out customerPriceList))
                {
                    customerPriceList = new CustomerPriceListChange();
                    customerPriceListChanges.Add(codeChange.PricelistId, customerPriceList);
                }
                customerPriceList.CodeChanges.Add(codeChange);
            }
            foreach (var rateChange in rateChanges)
            {
                CustomerPriceListChange customerPriceList;
                if (!customerPriceListChanges.TryGetValue(rateChange.PricelistId, out customerPriceList))
                {
                    customerPriceList = new CustomerPriceListChange();
                    customerPriceListChanges.Add(rateChange.PricelistId, customerPriceList);
                }
                customerPriceList.RateChanges.Add(rateChange);
            }

            var priceListByCustomerId = new Dictionary<int, List<CustomerPriceListChange>>();
            SalePriceListManager salePriceListManager = new SalePriceListManager();

            foreach (var customerPriceList in customerPriceListChanges)
            {
                var priceList = salePriceListManager.GetPriceList(customerPriceList.Key);
                List<CustomerPriceListChange> changes;
                if (!priceListByCustomerId.TryGetValue(priceList.OwnerId, out changes))
                {
                    changes = new List<CustomerPriceListChange>();
                    priceListByCustomerId.Add(priceList.OwnerId, changes);
                }
                var customerPriceListValue = customerPriceList.Value;
                customerPriceListValue.CustomerId = priceList.OwnerId;
                customerPriceListValue.PriceListId = priceList.PriceListId;
                changes.Add(customerPriceListValue);
            }
            return priceListByCustomerId;
        }
        #region Mapper
        private SalePricelistRateChange SalePricelistRateChangeDetailMapper(SalePricelistRateChange salePricelistRateChange)
        {
            return salePricelistRateChange;
        }
        private SalePricelistCodeChange SalePricelistCodeChangeDetailMapper(SalePricelistCodeChange salePricelistCodeChange)
        {
            return salePricelistCodeChange;
        }
        #endregion

    }
}
