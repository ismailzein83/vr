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

public class DemoCurrencyManager
    {
      
       #region Public Methods

    public string GetDemoCurrencyName(int DemoCurrencyId)
    {
        var parent = GetDemoCurrencyById(DemoCurrencyId);
        if (parent == null)
            return null;
        return parent.Name;
    }



    
    public DemoCurrency GetDemoCurrencyById(int demoCurrencyId)
        {
            var allDemoCurrencies = GetCachedDemoCurrencies();
            return allDemoCurrencies.GetRecord(demoCurrencyId);
        }

    public IEnumerable<DemoCurrencyInfo> GetDemoCurrenciesInfo(DemoCurrencyInfoFilter demoCurrencyInfoFilter)
    {
        var allDemoCurrencies = GetCachedDemoCurrencies();
        Func<DemoCurrency, bool> filterFunc = (demoCurrency) =>
        {
            if (demoCurrencyInfoFilter != null)
            {
                if (demoCurrencyInfoFilter.Filters != null)
                {
                    var context = new DemoCurrencyInfoFilterContext
                    {
                        DemoCurrencyId = demoCurrency.DemoCurrencyId
                    };
                    foreach (var filter in demoCurrencyInfoFilter.Filters)
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
        return allDemoCurrencies.MapRecords(DemoCurrencyInfoMapper, filterFunc).OrderBy(demoCurrency => demoCurrency.Name);
    }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDemoCurrencyDataManager demoCurrencyDataManager = DemoModuleFactory.GetDataManager<IDemoCurrencyDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return demoCurrencyDataManager.AreDemoCurrenciesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<int, DemoCurrency> GetCachedDemoCurrencies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedDemoCurrencies", () =>
               {
                   IDemoCurrencyDataManager demoCurrencyDataManager = DemoModuleFactory.GetDataManager<IDemoCurrencyDataManager>();
                   List<DemoCurrency> demoCurrencies = demoCurrencyDataManager.GetDemoCurrencies();
                   return demoCurrencies.ToDictionary(demoCurrency => demoCurrency.DemoCurrencyId, demoCurrency => demoCurrency);
               });
        }
        #endregion
    
        #region Mappers
        public DemoCurrencyDetails DemoCurrencyDetailMapper(DemoCurrency demoCurrency)
        {
            var demoCurrencyDetails = new DemoCurrencyDetails
            {
                Name = demoCurrency.Name,
                DemoCurrencyId = demoCurrency.DemoCurrencyId,
              
            };

            return demoCurrencyDetails;
        }

        public DemoCurrencyInfo DemoCurrencyInfoMapper(DemoCurrency demoCurrency)
        {
            return new DemoCurrencyInfo
            {
                Name = demoCurrency.Name,
                DemoCurrencyId = demoCurrency.DemoCurrencyId
            };
        }
        #endregion 
    
}
