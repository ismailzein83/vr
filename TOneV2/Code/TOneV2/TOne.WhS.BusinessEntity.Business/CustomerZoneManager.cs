using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerZoneManager
    {
     
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public CustomerZones GetCustomerZones(int customerId, DateTime? effectiveOn, bool futureEntities)
        {
            CustomerZones customerZones = null;
            var cached = this.GetAllCachedCustomerZones();

            var filtered = cached.FindAllRecords
            (
                x => x.CustomerId == customerId
                && (effectiveOn == null || (x.StartEffectiveTime == null || x.StartEffectiveTime <= effectiveOn))
            );

            if (filtered != null)
            {
                var ordered = filtered.OrderByDescending(item => item.StartEffectiveTime);
                customerZones = ordered.FirstOrDefault();
            }

            return customerZones;
        }
        public IEnumerable<Vanrise.Entities.Country> GetCountriesToSell(int customerId)
        {
            IEnumerable<Vanrise.Entities.Country> countriesToSell = null;

            var allCountries = new CountryManager().GetCachedCountries();
            CustomerZones customerZones = this.GetCustomerZones(customerId, DateTime.Now, false);

            if (customerZones != null)
            {
                countriesToSell = new List<Vanrise.Entities.Country>();
                IEnumerable<int> customerCountryIds = customerZones.Countries.MapRecords(c => c.CountryId);
                countriesToSell = allCountries.FindAllRecords(c => !customerCountryIds.Contains(c.CountryId));
            }
            else
            {
                countriesToSell = allCountries.Values;
            }

            return countriesToSell;
        }
        public IEnumerable<SaleZone> GetCustomerSaleZones(int customerId, DateTime effectiveOn, bool futureEntities)
        {
            IEnumerable<SaleZone> saleZones = null;
            CustomerZones customerZones = GetCustomerZones(customerId, effectiveOn, futureEntities);

            if (customerZones != null)
            {
                int sellingNumberPlanId = new CarrierAccountManager().GetSellingNumberPlanId(customerId, CarrierAccountType.Customer);
                IEnumerable<int> countryIds = customerZones.Countries.MapRecords(item => item.CountryId);
                saleZones = new SaleZoneManager().GetSaleZonesByCountryIds(sellingNumberPlanId, countryIds);
                saleZones = saleZones.FindAllRecords(itm => itm.IsEffective(effectiveOn));
            }

            return saleZones;
        }
        public TOne.Entities.InsertOperationOutput<CustomerZones> AddCustomerZones(CustomerZones customerZones)
        {
            CustomerZones currentCustomerZones = this.GetCustomerZones(customerZones.CustomerId, DateTime.Now, false);

            if (currentCustomerZones != null)
            {
                foreach (CustomerCountry country in currentCustomerZones.Countries)
                {
                    customerZones.Countries.Add(new CustomerCountry()
                    {
                        CountryId = country.CountryId,
                        StartEffectiveTime = country.StartEffectiveTime
                    });
                }
            }

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
        #endregion

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICustomerZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAllCustomerZonesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, CustomerZones> GetAllCachedCustomerZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllCustomerZones",
               () =>
               {
                   ICustomerZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerZoneDataManager>();
                   IEnumerable<CustomerZones> customerZones = dataManager.GetAllCustomerZones();
                   return customerZones.ToDictionary(kvp => kvp.CustomerZonesId, kvp => kvp);
               });
        }

        #endregion
    
        #region  Mappers
        #endregion
    }
}
