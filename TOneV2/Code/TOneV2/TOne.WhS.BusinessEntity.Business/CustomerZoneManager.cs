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

        public List<Country> GetCountriesToSell(int customerId)
        {
            CountryManager countryManager = new CountryManager();
            List<Country> countriesToSell = countryManager.GetAllCountries().ToList();
            
            CustomerZones customerZones = GetCustomerZones(customerId, DateTime.Now, false);

            if (customerZones != null && customerZones.Countries != null && customerZones.Countries.Count > 0)
            {
                List<int> customerCountryIds = customerZones.Countries.Select(x => x.CountryId).ToList();
                countriesToSell = countriesToSell.Where(x => !customerCountryIds.Contains(x.CountryId)).ToList();
            }

            return countriesToSell;
        }

        public List<char> GetCustomerZoneLetters(int customerId)
        {
            List<char> customerZoneLetters = new List<char>();
            List<SaleZone> customerSaleZones = GetCustomerSaleZones(customerId);

            if (customerSaleZones != null)
            {
                List<string> customerSaleZoneNames = customerSaleZones.Select(x => x.Name).ToList();
                customerZoneLetters = customerSaleZoneNames.Where(x => x != null && x.Length > 0).Select(x => x[0]).OrderBy(x => x).Distinct().ToList();
            }

            return customerZoneLetters;
        }

        public TOne.Entities.InsertOperationOutput<CustomerZones> AddCustomerZones(CustomerZones customerZones)
        {
            CustomerZones currentCustomerZones = GetCustomerZones(customerZones.CustomerId, DateTime.Now, false);

            if (currentCustomerZones != null)
            {
                foreach (Country country in currentCustomerZones.Countries)
                {
                    customerZones.Countries.Add(new Country()
                    {
                        CountryId = country.CountryId,
                        Name = country.Name
                    });
                }
            }

            customerZones.Countries = customerZones.Countries.OrderBy(x => x.Name).ToList();

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

        public List<SaleZone> GetCustomerSaleZones(int customerId, DateTime effectiveOn, bool futureEntities)
        {
            List<SaleZone> customerSaleZones = null;
            CustomerZones customerZones = GetCustomerZones(customerId, effectiveOn, futureEntities);
            
            if (customerZones != null && customerZones.Countries != null && customerZones.Countries.Count > 0)
            {
                List<int> countryIds = customerZones.Countries.Select(x => x.CountryId).ToList();

                SaleZoneManager saleZoneManager = new SaleZoneManager();
                customerSaleZones = saleZoneManager.GetSaleZonesByCountryIds(GetSellingNumberPlanId(customerId), countryIds);
            }

            return customerSaleZones;
        }

        private List<SaleZone> GetCustomerSaleZones(int customerId)
        {
            return GetCustomerSaleZones(customerId, DateTime.Now, false);
        }

        private int GetSellingNumberPlanId(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccountDetail customer = manager.GetCarrierAccount(customerId);

            return customer.CustomerSettings.SellingNumberPlanId;
        }

        #endregion
    }
}
