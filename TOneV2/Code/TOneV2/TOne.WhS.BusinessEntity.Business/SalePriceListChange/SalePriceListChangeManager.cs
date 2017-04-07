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
            SellingProductManager sellingProductManager = new SellingProductManager();
            var priceList = salePriceListManager.GetPriceList(priceListId);
            if (priceList != null)
            {
                var ownerId = priceList.OwnerId;
                ownerName = priceList.OwnerType == SalePriceListOwnerType.Customer
                    ? carrierAccountManager.GetCarrierAccountName(ownerId)
                    : sellingProductManager.GetSellingProductName(ownerId);
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
                        Rate = decimal.Round(rate.Rate, 8),
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
        public Dictionary<int, List<CustomerPriceListChange>> GetNotSentChanges(IEnumerable<int> customerIds)
        {
            var manager = new SalePriceListManager();
            var customerPriceListChanges = new Dictionary<int, List<CustomerPriceListChange>>();
            ISalePriceListChangeDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            List<SalePricelistCodeChange> codeChanges = dataManager.GetNotSentCodechanges(customerIds);
            List<SalePricelistRateChange> rateChanges = dataManager.GetNotSentRatechanges(customerIds);
            var codeChangesByPriceListId =
                codeChanges.GroupBy(r => r.PricelistId)
                    .Select(group => new { PriceListId = @group.Key, Items = @group.ToList() });

            var rateChangesByPriceListId =
              rateChanges.GroupBy(r => r.PricelistId)
                  .Select(group => new { PriceListId = @group.Key, Items = @group.ToList() });
            foreach (var grouppedCodes in codeChangesByPriceListId)
            {
                CustomerPriceListChange customerPriceList = GetCustomerPriceListChange(manager, grouppedCodes.PriceListId);
                List<CustomerPriceListChange> customerPriceListChangesTemp;
                if (!customerPriceListChanges.TryGetValue(customerPriceList.CustomerId, out customerPriceListChangesTemp))
                {
                    customerPriceListChangesTemp = new List<CustomerPriceListChange>();
                    customerPriceListChanges.Add(customerPriceList.CustomerId, customerPriceListChangesTemp);
                }
                customerPriceList.CodeChanges.AddRange(grouppedCodes.Items);
                customerPriceListChangesTemp.Add(customerPriceList);
            }
            foreach (var grouppedRateChange in rateChangesByPriceListId)
            {
                var customerPriceList = GetCustomerPriceListChange(manager, grouppedRateChange.PriceListId);
                if (customerPriceList == null) continue;
                List<CustomerPriceListChange> customerPriceListChangesTemp;
                if (!customerPriceListChanges.TryGetValue(customerPriceList.CustomerId, out customerPriceListChangesTemp))
                {
                    customerPriceListChangesTemp = new List<CustomerPriceListChange>();
                    customerPriceListChanges.Add(customerPriceList.CustomerId, customerPriceListChangesTemp);
                }
                customerPriceList.RateChanges.AddRange(grouppedRateChange.Items);
                customerPriceListChangesTemp.Add(customerPriceList);
            }
            return customerPriceListChanges;
        }

        private CustomerPriceListChange GetCustomerPriceListChange(SalePriceListManager manager, int pricelistId)
        {
            var priceList = manager.GetPriceList(pricelistId);
            if (priceList != null)
                return new CustomerPriceListChange
                {
                    CustomerId = priceList.OwnerId,
                    PriceListId = priceList.PriceListId
                };
            return null;
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
