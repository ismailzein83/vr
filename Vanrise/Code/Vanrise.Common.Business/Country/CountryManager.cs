using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class CountryManager : IBusinessEntityManager
    {

        public Vanrise.Entities.IDataRetrievalResult<CountryDetail> GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryQuery> input)
        {
            var allCountries = GetCachedCountries();

            Func<Country, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCountries.ToBigResult(input, filterExpression, CountryDetailMapper));
        }

        public IEnumerable<CountryInfo> GeCountriesInfo()
        {
            return this.GetCachedCountries().MapRecords(CountryInfoMapper).OrderBy(x => x.Name);
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

        public string GetCountryName(int countryId)
        {
            var country = GetCountry(countryId);
            if (country != null)
                return country.Name;
            else
                return null;
        }

        public Country GetCountry(string countryName)
        {
            if (countryName == null)
                return null;
            var countries = GetCachedCountriesByNames();
            return countries.GetRecord(countryName.ToLower());
        }

        public Country GetCountryBySourceId(string sourceId)
        {
            return GetCachedCountries().FindRecord(itm => itm.SourceId == sourceId);
        }

        public string AddCountries(int fileId)
        {
            DataTable countryDataTable = new DataTable();
            VRFileManager fileManager = new VRFileManager();
            byte[] bytes = fileManager.GetFile(fileId).Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            ExportTableOptions options = new ExportTableOptions();
            options.CheckMixedValueType = true;
            Workbook wbk = new Workbook(fileStream);
            wbk.CalculateFormula();
            string message = "";
            int insertedCount = 0 ;
            int notInsertedCount = 0;

            if (wbk.Worksheets[0].Cells.MaxDataRow > -1 && wbk.Worksheets[0].Cells.MaxDataColumn > -1)
                countryDataTable = wbk.Worksheets[0].Cells.ExportDataTableAsString(0, 0, wbk.Worksheets[0].Cells.MaxDataRow + 1, wbk.Worksheets[0].Cells.MaxDataColumn + 1);
            
            for (int i = 1; i < countryDataTable.Rows.Count; i++)
            {
                string importedCountryName = countryDataTable.Rows[i][0].ToString().Trim();
                Country country = GetCachedCountries().FindRecord(it => it.Name.Equals(importedCountryName, StringComparison.InvariantCultureIgnoreCase));
                if(!String.IsNullOrEmpty(importedCountryName))
                {
                    if (country == null)
                    {
                        country = new Country();
                        long startingId;
                        ReserveIDRange(1, out startingId);
                        country.CountryId = (int)startingId;
                        country.Name = importedCountryName;

                        ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
                        bool insertActionSucc = dataManager.Insert(country);
                        if (insertActionSucc)
                            insertedCount++;
                        else                           
                            notInsertedCount++;
                    }
                    else
                    {
                        notInsertedCount++;
                    }
                }
                
               
            }
           
            message = String.Format("{0} countries added and {1} are already exists", insertedCount, notInsertedCount);

            return message;
        }
        public Vanrise.Entities.InsertOperationOutput<CountryDetail> AddCountry(Country country)
        {
            Vanrise.Entities.InsertOperationOutput<CountryDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<CountryDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            long startingId;
            ReserveIDRange(1, out startingId);
            country.CountryId = (int)startingId;

            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            bool insertActionSucc = dataManager.Insert(country);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = CountryDetailMapper(country);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public void AddCountryFromSource(Country country)
        {
            long startingId;
            ReserveIDRange(1, out startingId);
            country.CountryId = (int)startingId;

            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            dataManager.InsertFromSource(country);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

        }

        public void UpdateCountryFromSource(Country country)
        {

            ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
            dataManager.UpdateFromSource(country);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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

        public List<Vanrise.Entities.TemplateConfig> GetCountrySourceTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SourceCountryReaderConfigType);
        }

        public Dictionary<string, long> GetExistingItemIds()
        {
            Dictionary<string, long> existingItemIds = new Dictionary<string, long>();
            foreach (var item in GetCachedCountries())
            {
                if (item.Value.SourceId != null)
                {
                    //if (sourceItemIds.Contains(item.Value.SourceId))
                        existingItemIds.Add(item.Value.SourceId, (long)item.Value.CountryId);
                }
            }
            return existingItemIds;
        }

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            var countriesNames = new List<string>();
            foreach (var entityId in context.EntityIds)
            {
                string countryName = GetCountryName(Convert.ToInt32(entityId));
                if (countryName == null) throw new NullReferenceException("countryName");
                countriesNames.Add(countryName);
            }
            return String.Join(",", countriesNames);
        }
        public bool IsMatched(IBusinessEntityMatchContext context)
        {
            if (context.FieldValueIds == null || context.FilterIds == null) return true;

            var fieldValueIds = context.FieldValueIds.MapRecords(itm => Convert.ToInt32(itm));
            var filterIds = context.FilterIds.MapRecords(itm => Convert.ToInt32(itm));
            foreach (var filterId in filterIds)
            {
                if (fieldValueIds.Contains(filterId))
                    return true;
            }
            return false;
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
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCountriesByName",
               () =>
               {
                   ICountrytDataManager dataManager = CommonDataManagerFactory.GetDataManager<ICountrytDataManager>();
                   IEnumerable<Country> countries = dataManager.GetCountries();
                   Dictionary<string, Country> dic = new Dictionary<string, Country>();
                   if (countries != null)
                   {
                       foreach (var c in countries)
                       {
                           if (!dic.ContainsKey(c.Name.ToLower()))
                               dic.Add(c.Name.ToLower(), c);
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

        private CountryInfo CountryInfoMapper(Country country)
        {
            CountryInfo countryInfo = new CountryInfo();

            countryInfo.CountryId = country.CountryId;
            countryInfo.Name = country.Name;
            return countryInfo;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(CountryManager), nbOfIds, out startingId);
        }

        #endregion
        
        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllCountries().Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }


        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetCountry(context.EntityId);
        }
    }
}
