﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class CountryManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CountryDetail> GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryQuery> input)
        {
            var allCountries = GetCachedCountries();

            Func<Country, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCountries.ToBigResult(input, filterExpression, CountryDetailMapper));     
        }

        public IEnumerable<Country> GetAllCountries()
        {
            var allCountries = GetCachedCountries();
            if (allCountries == null)
                return null;

            return allCountries.Values;
        }
        public Country GetCountry(int countryId)
        {
            var countries = GetCachedCountries();
            return countries.GetRecord(countryId);
        }

        public Country GetCountry(string countryName)
        {
            var countries = GetCachedCountriesByNames();
            return countries.GetRecord(countryName);
        }
        public Vanrise.Entities.InsertOperationOutput<CountryDetail> AddCountry(Country country)
        {
            Vanrise.Entities.InsertOperationOutput<CountryDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CountryDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int countryId = -1;

            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            bool insertActionSucc = dataManager.Insert(country, out countryId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                country.CountryId = countryId;
                insertOperationOutput.InsertedObject = CountryDetailMapper(country);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<CountryDetail> UpdateCountry(Country country)
        {
            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();

            bool updateActionSucc = dataManager.Update(country);
            Vanrise.Entities.UpdateOperationOutput<CountryDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<CountryDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = CountryDetailMapper(country);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #region Private Members

        public Dictionary<int, Country> GetCachedCountries()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCountries",
               () =>
               {
                   ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
                   IEnumerable<Country> countries = dataManager.GetCountries();
                   return countries.ToDictionary(cn => cn.CountryId, cn => cn);
               });
        }

        public Dictionary<string, Country> GetCachedCountriesByNames()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCountries",
               () =>
               {
                   ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
                   IEnumerable<Country> countries = dataManager.GetCountries();
                   Dictionary<string, Country> dic = new Dictionary<string, Country>();
                   if(countries != null)
                   {
                       foreach(var c in countries)
                       {
                           if (!dic.ContainsKey(c.Name))
                               dic.Add(c.Name, c);
                       }
                   }
                   return dic;
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICountrytDataManager _dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCountriesUpdated(ref _updateHandle);
            }
        }

        private CountryDetail CountryDetailMapper(Country country)
        {
            CountryDetail countryDetail = new CountryDetail();

            countryDetail.Entity = country;            
            return countryDetail;
        }
        #endregion
    }
}
