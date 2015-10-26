using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneManager
    {
        public Dictionary<int, CustomerZones> GetAllCachedCustomerZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllCustomerZones",
               () =>
               {
                   ICustomerZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerZoneDataManager>();
                   IEnumerable<CustomerZones> customerZones = dataManager.GetAllCustomerZones();
                   return customerZones.ToDictionary(kvp => kvp.CustomerZonesId, kvp => kvp);
               });
        }

        public CustomerZones GetCustomerZones(int customerId, DateTime? effectiveOn, bool futureEntities)
        {
            var allCustomerZones = GetAllCachedCustomerZones();

            if (allCustomerZones.Count > 0)
            {
                var filteredCustomerZones = allCustomerZones.Values.Where(x => x.CustomerId == customerId && x.StartEffectiveTime <= effectiveOn);
                
                if (filteredCustomerZones != null && filteredCustomerZones.Count() > 0)
                    return filteredCustomerZones.OrderByDescending(x => x.StartEffectiveTime).FirstOrDefault();
            }

            return null;
        }

        public Vanrise.Entities.IDataRetrievalResult<SaleZoneDetail> GetFilteredSaleZonesToSell(Vanrise.Entities.DataRetrievalInput<SaleZonesToSellQuery> input)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZoneDetail> allSaleZones = saleZoneManager.GetCachedSaleZoneDetails(GetSellingNumberPlanId(input.Query.CustomerId));
            CustomerZones customerZones = GetCustomerZones(input.Query.CustomerId, DateTime.Now, false);

            Func<SaleZoneDetail, bool> filterExpression = (saleZone) => (input.Query.Name == null || saleZone.Entity.Name.Contains(input.Query.Name));

            if (customerZones != null) {
                List<long> customerZoneIds = customerZones.Zones.Select(x => x.ZoneId).ToList();
                filterExpression = (saleZone) =>
                    (
                        !customerZoneIds.Contains(saleZone.Entity.SaleZoneId)
                    )
                    &&
                    (
                        input.Query.Name == null || saleZone.Entity.Name.Contains(input.Query.Name)
                    );
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSaleZones.ToBigResult(input, filterExpression));
        }

        public TOne.Entities.InsertOperationOutput<CustomerZones> AddCustomerZones(CustomerZones customerZones)
        {
            CustomerZones currentCustomerZones = GetCustomerZones(customerZones.CustomerId, DateTime.Now, false);

            if (currentCustomerZones != null)
            {
                foreach (CustomerZone customerZone in currentCustomerZones.Zones)
                {
                    customerZones.Zones.Add(new CustomerZone() {
                        ZoneId = customerZone.ZoneId
                    });
                }
            }

            customerZones.Zones.OrderBy(x => x.ZoneId); // should i keep this?

            TOne.Entities.InsertOperationOutput<CustomerZones> insertOperationOutput = new TOne.Entities.InsertOperationOutput<CustomerZones>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;

            int customerZonesId = -1;

            ICustomerZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerZoneDataManager>();
            bool inserted = dataManager.AddCustomerZones(customerZones, out customerZonesId);

            if (inserted)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                customerZones.CustomerZonesId = customerZonesId;
                insertOperationOutput.InsertedObject = customerZones;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
            }

            return insertOperationOutput;
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICustomerZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAllCustomerZonesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private int GetSellingNumberPlanId(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccountDetail customer = manager.GetCarrierAccount(customerId);

            return customer.CustomerSettings.SellingNumberPlanId;
        }

        #endregion
    }
}
