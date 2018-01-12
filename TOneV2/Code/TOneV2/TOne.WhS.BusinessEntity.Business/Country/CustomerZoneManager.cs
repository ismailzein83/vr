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
        #region Public Methods

        public CustomerZones GetCustomerZones(int customerId, DateTime? effectiveOn, bool futureEntities)
        {
            Dictionary<int, CustomerZones> cachedEntities = this.GetAllCachedCustomerZones();
            Func<CustomerZones, bool> filterFunc = (entity) =>
            {
                if (entity.CustomerId != customerId)
                    return false;
                if (effectiveOn != null && entity.StartEffectiveTime > effectiveOn)
                    return false;
                return true;
            };
            IEnumerable<CustomerZones> filteredEntities = cachedEntities.FindAllRecords(filterFunc).OrderByDescending(x => x.CustomerZonesId);
            return filteredEntities.FirstOrDefault();
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

        public IEnumerable<SaleZone> GetCustomerSaleZones(int customerId, int sellingNumberPlanId, DateTime effectiveOn, bool withFutureZones)
        {
            CustomerZones customerZones = GetCustomerZones(customerId, effectiveOn, withFutureZones);
            if (customerZones != null)
            {
                IEnumerable<int> countryIds = customerZones.Countries.MapRecords(x => x.CountryId);
                return new SaleZoneManager().GetSaleZonesByCountryIds(sellingNumberPlanId, countryIds, effectiveOn, withFutureZones);
            }
            return null;
        }

        public InsertOperationOutput<CustomerZones> AddCustomerZones(CustomerZones customerZones)
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

            InsertOperationOutput<CustomerZones> insertOperationOutput = new InsertOperationOutput<CustomerZones>();
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

        public IEnumerable<int> GetCustomerIdsByCountryId(IEnumerable<int> customerIds, int countryId)
        {
            if (customerIds == null)
                return null;
            Func<int, bool> filterFunc = (customerId) =>
            {
                CustomerZones customerZones = GetCustomerZones(customerId, DateTime.Now, false);
                if (customerZones == null || customerZones.Countries == null)
                    return false;
                if (!customerZones.Countries.Any(x => x.CountryId == countryId))
                    return false;
                return true;
            };
            return customerIds.FindAllRecords(filterFunc);
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
    }

    public class CustomerCountryManager
    {
        #region Public Methods

        public Dictionary<int, DateTime> GetSoldCountryDatesByCountryId(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var datesByCountry = new Dictionary<int, DateTime>();
            var customerCountryManager = new CustomerCountryManager();
            IEnumerable<CustomerCountry2> customerCountries;

            if (isEffectiveInFuture)
            {
                if (!effectiveOn.HasValue)
                    throw new ArgumentException("Cannot find effective and future countries without an effective date");

                customerCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(customerId, effectiveOn.Value);
            }
            else
            {
                customerCountries = customerCountryManager.GetCustomerCountries(customerId, effectiveOn, false);
            }
            if (customerCountries != null)
            {
                foreach (CustomerCountry2 customerCountry in customerCountries)
                    if (!datesByCountry.ContainsKey(customerCountry.CountryId))
                        datesByCountry.Add(customerCountry.CountryId, customerCountry.BED);
            }
            return datesByCountry;
        }
        public CustomerCountry2 GetCustomerCountry(int customerId, int countryId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var cachedCustomerCountriesByCustomerCountry = GetAllCachedCustomerCountriesByCustomerCountry();
            if (cachedCustomerCountriesByCustomerCountry == null)
                return null;

            IOrderedEnumerable<CustomerCountry2> customerCountries = cachedCustomerCountriesByCustomerCountry.GetRecord(new CustomerCountryDefinition() { CustomerId = customerId, CountryId = countryId });
            if (customerCountries != null)
                return customerCountries.FindAllRecords(x => x.IsEffective(effectiveOn, isEffectiveInFuture)).FirstOrDefault();
            else
                return null;
        }

        public CustomerCountry2 GetEffectiveOrFutureCustomerCountry(int customerId, int countryId, DateTime date)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            if (customerCountries != null)
                return customerCountries.FindAllRecords(x => x.CountryId == countryId && x.IsEffectiveOrFuture(date)).FirstOrDefault();
            else
                return null;
        }

        public IEnumerable<CustomerCountry2> GetCustomerCountries(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            if (customerCountries == null)
                return null;
            return customerCountries.FindAllRecords(x => x.IsEffective(effectiveOn, isEffectiveInFuture));
        }

        public IEnumerable<CustomerCountry2> GetEffectiveOrFutureCustomerCountries(int customerId, DateTime date)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            return (customerCountries != null) ? customerCountries.FindAllRecords(x => x.IsEffectiveOrFuture(date)) : null;
        }

        public IEnumerable<CustomerCountry2> GetSoldCountries(int customerId, DateTime effectiveOn)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            if (customerCountries == null)
                return null;
            return customerCountries.FindAllRecords(x => x.IsEffective(effectiveOn) || x.IsEffective(null, true));
        }

        public IEnumerable<int> GetCustomerCountryIds(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId, effectiveOn, isEffectiveInFuture);
            if (customerCountries == null)
                return null;
            return customerCountries.MapRecords(x => x.CountryId).Distinct();
        }

        public IEnumerable<int> GetCountryIdsEffectiveAfterByCustomer(int customerId, DateTime effectiveOn)
        {
            IEnumerable<CustomerCountry2> countries = GetCustomerCountries(customerId);
            if (countries != null)
            {
                IEnumerable<CustomerCountry2> countriesEffectiveAfter = countries.FindAllRecords(x => x.IsEffectiveOrFuture(effectiveOn));
                return countriesEffectiveAfter.MapRecords(x => x.CountryId).Distinct();
            }
            return null;
        }

        public bool IsCountrySoldToCustomer(int customerId, int countryId, DateTime effectiveOn)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            if (customerCountries == null)
                return false;
            return customerCountries.Any(x => x.CountryId == countryId && (x.IsEffective(effectiveOn) || x.IsEffective(null, true)));
        }
        public IEnumerable<CustomerCountry2> GetNotClosedCustomerCountries(int customerId)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            return customerCountries != null ? customerCountries.FindAllRecords(x => x.EED == null) : null;
        }
        public IEnumerable<CustomerCountry2> GetCustomerCountriesEffectiveAfter(int customerId, DateTime date)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            if (customerCountries != null)
                return customerCountries.FindAllRecords(x => x.IsEffectiveOrFuture(date));
            else
                return null;
        }
        public IEnumerable<CustomerCountry2> GetNotEndedCustomerCountriesEffectiveAfter(int customerId, DateTime date)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            if (customerCountries != null)
                return customerCountries.FindAllRecords(x => x.IsEffectiveOrFuture(date) && !x.EED.HasValue);
            else
                return null;
        }
        public IEnumerable<CustomerCountry2> GetCustomerCountriesEffectiveAfter(int customerId, DateTime date, long? processInstanceId)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountriesEffectiveAfter(customerId, date);
            return customerCountries != null ? customerCountries.FindAllRecords(x => (x.ProcessInstanceId == null || x.ProcessInstanceId <= processInstanceId)) : null;
        }

        public long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(this.GetCustomerCountryType(), numberOfIds, out startingId);
            return startingId;
        }

        public int GetCustomerCountryTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetCustomerCountryType());
        }

        public IEnumerable<CustomerCountry2> GetCustomerCountries(int customerId)
        {
            Dictionary<int, List<CustomerCountry2>> allCustomerCountries = GetAllCachedCustomerCountriesByCustomer();
            return allCustomerCountries.GetRecord(customerId);
        }

        public bool AreEffectiveOrFutureCountriesSoldToCustomer(int customerId, DateTime date)
        {
            IEnumerable<CustomerCountry2> customerCountries = GetCustomerCountries(customerId);
            return (customerCountries != null && customerCountries.Any(x => x.IsEffectiveOrFuture(date)));
        }

        public IEnumerable<CustomerCountry2> GetEndedCustomerCountriesEffectiveAfter(int customerId, DateTime effectiveOn)
        {
            Dictionary<int, List<CustomerCountry2>> countriesByCustomerId = GetAllCachedCustomerCountriesByCustomer();
            List<CustomerCountry2> customerCountries;
            if (countriesByCustomerId.TryGetValue(customerId, out customerCountries))
                return customerCountries.FindAllRecords(x => x.EED.HasValue && x.IsEffectiveOrFuture(effectiveOn));
            return null;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICustomerCountryDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICustomerCountryDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAllCustomerCountriesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, List<CustomerCountry2>> GetAllCachedCustomerCountriesByCustomer()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllCustomerCountriesByCustomer", () =>
            {
                var cachedCustomerCountries = GetAllCachedCustomerCountries();

                var countriesByCustomer = new Dictionary<int, List<CustomerCountry2>>();
                foreach (CustomerCountry2 customerCountry in cachedCustomerCountries.Values)
                {
                    if (customerCountry.EED <= customerCountry.BED)
                        continue;
                    List<CustomerCountry2> customerCountries;
                    if (!countriesByCustomer.TryGetValue(customerCountry.CustomerId, out customerCountries))
                    {
                        customerCountries = new List<CustomerCountry2>();
                        countriesByCustomer.Add(customerCountry.CustomerId, customerCountries);
                    }
                    customerCountries.Add(customerCountry);
                }
                return countriesByCustomer;
            });
        }

        private Dictionary<CustomerCountryDefinition, IOrderedEnumerable<CustomerCountry2>> GetAllCachedCustomerCountriesByCustomerCountry()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllCustomerCountriesByCustomerCountry", () =>
            {
                var cachedCustomerCountries = GetAllCachedCustomerCountries();

                var countriesByCustomer = new Dictionary<CustomerCountryDefinition, List<CustomerCountry2>>();
                foreach (CustomerCountry2 customerCountry in cachedCustomerCountries.Values)
                {
                    if (customerCountry.EED <= customerCountry.BED)
                        continue;
                    CustomerCountryDefinition customerCountryDefinition = new CustomerCountryDefinition() { CountryId = customerCountry.CountryId, CustomerId = customerCountry.CustomerId };
                    List<CustomerCountry2> customerCountries = countriesByCustomer.GetOrCreateItem(customerCountryDefinition);
                    customerCountries.Add(customerCountry);
                }
                return countriesByCustomer.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(item => item.BED));
            });
        }

        private Dictionary<int, CustomerCountry2> GetAllCachedCustomerCountries()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllCustomerCountries", () =>
            {
                var dataManager = BEDataManagerFactory.GetDataManager<ICustomerCountryDataManager>();
                IEnumerable<CustomerCountry2> allCustomerCountries = dataManager.GetAll();
                return allCustomerCountries.ToDictionary(cc => cc.CustomerCountryId, cc => cc);
            });
        }

        private Type GetCustomerCountryType()
        {
            return this.GetType();
        }


        private struct CustomerCountryDefinition
        {
            public int CustomerId { get; set; }
            public int CountryId { get; set; }
        }
        #endregion

        #region Pending Methods
        /*
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

		    public IEnumerable<SaleZone> GetCustomerSaleZones(int customerId, int sellingNumberPlanId, DateTime effectiveOn, bool withFutureZones)
		    {
			    CustomerZones customerZones = GetCustomerZones(customerId, effectiveOn, withFutureZones);
			    if (customerZones != null)
			    {
				    IEnumerable<int> countryIds = customerZones.Countries.MapRecords(x => x.CountryId);
				    return new SaleZoneManager().GetSaleZonesByCountryIds(sellingNumberPlanId, countryIds, effectiveOn, withFutureZones);
			    }
			    return null;
		    }

		    public IEnumerable<int> GetCustomerIdsByCountryId(IEnumerable<int> customerIds, int countryId)
		    {
			    if (customerIds == null)
				    return null;
			    Func<int, bool> filterFunc = (customerId) =>
			    {
				    CustomerZones customerZones = GetCustomerZones(customerId, DateTime.Now, false);
				    if (customerZones == null || customerZones.Countries == null)
					    return false;
				    if (!customerZones.Countries.Any(x => x.CountryId == countryId))
					    return false;
				    return true;
			    };
			    return customerIds.FindAllRecords(filterFunc);
		    }
		    */
        #endregion
    }
}
