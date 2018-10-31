using Demo.Module.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Demo.Module.Entities;
using Demo.Module.Entities.ProductInfo;

public class DemoCountryManager
    {
      
       #region Public Methods
    public IDataRetrievalResult<DemoCountryDetails> GetFilteredDemoCountries(DataRetrievalInput<DemoCountryQuery> input)
        {
            var allDemoCountries = GetCachedDemoCountries();
            Func<DemoCountry, bool> filterExpression = (demoCountry) =>
            {
                if (input.Query.Name != null && !demoCountry.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allDemoCountries.ToBigResult(input, filterExpression, DemoCountryDetailMapper));

        }

    public string GetDemoCountryName(int demoCountryId)
    {
        var parent = GetDemoCountryById(demoCountryId);
        if (parent == null)
            return null;
        return parent.Name;
    }



    public InsertOperationOutput<DemoCountryDetails> AddDemoCountry(DemoCountry demoCountry)
        {
            IDemoCountryDataManager demoCountryDataManager = DemoModuleFactory.GetDataManager<IDemoCountryDataManager>();
            InsertOperationOutput<DemoCountryDetails> insertOperationOutput = new InsertOperationOutput<DemoCountryDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int demoCountryId = -1;

            bool insertActionSuccess = demoCountryDataManager.Insert(demoCountry, out demoCountryId);
            if (insertActionSuccess)
            {
                demoCountry.DemoCountryId = demoCountryId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = DemoCountryDetailMapper(demoCountry);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
    public DemoCountry GetDemoCountryById(int demoCountryId)
        {
            var allDemoCountries = GetCachedDemoCountries();
            return allDemoCountries.GetRecord(demoCountryId);
        }

    public IEnumerable<DemoCountryInfo> GetDemoCountriesInfo(DemoCountryInfoFilter demoCountryInfoFilter)
    {
        var allDemoCountries = GetCachedDemoCountries();
        Func<DemoCountry, bool> filterFunc = (demoCountry) =>
        {
            if (demoCountryInfoFilter != null)
            {
                if (demoCountryInfoFilter.Filters != null)
                {
                    var context = new DemoCountryInfoFilterContext
                    {
                        DemoCountryId = demoCountry.DemoCountryId
                    };
                    foreach (var filter in demoCountryInfoFilter.Filters)
                    {
                        if (!filter.IsMatch(context))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        };
        return allDemoCountries.MapRecords(DemoCountryInfoMapper, filterFunc).OrderBy(demoCountry => demoCountry.Name);
    }

    public UpdateOperationOutput<DemoCountryDetails> UpdateDemoCountry(DemoCountry demoCountry)
        {
            IDemoCountryDataManager demoCountryDataManager = DemoModuleFactory.GetDataManager<IDemoCountryDataManager>();
            UpdateOperationOutput<DemoCountryDetails> updateOperationOutput = new UpdateOperationOutput<DemoCountryDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = demoCountryDataManager.Update(demoCountry);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DemoCountryDetailMapper(demoCountry);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDemoCountryDataManager demoCountryDataManager = DemoModuleFactory.GetDataManager<IDemoCountryDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return demoCountryDataManager.AreDemoCountriesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, DemoCountry> GetCachedDemoCountries()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedDemoCountries", () =>
               {
                   IDemoCountryDataManager demoCountryDataManager = DemoModuleFactory.GetDataManager<IDemoCountryDataManager>();
                   List<DemoCountry> demoCountries = demoCountryDataManager.GetDemoCountries();
                   return demoCountries.ToDictionary(demoCountry => demoCountry.DemoCountryId, demoCountry => demoCountry);
               });
        }
        #endregion
    
        #region Mappers
        public DemoCountryDetails DemoCountryDetailMapper(DemoCountry demoCountry)
        {
            var demoCountryDetails = new DemoCountryDetails
            {
                Name = demoCountry.Name,
                DemoCountryId = demoCountry.DemoCountryId,
              
            };

            return demoCountryDetails;
        }

        public DemoCountryInfo DemoCountryInfoMapper(DemoCountry demoCountry)
        {
            return new DemoCountryInfo
            {
                Name = demoCountry.Name,
                DemoCountryId = demoCountry.DemoCountryId
            };
        }
        #endregion 
    
}
