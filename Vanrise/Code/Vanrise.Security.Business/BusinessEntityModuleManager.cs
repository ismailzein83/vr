using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class BusinessEntityModuleManager
    {
        #region Public Methods

        public IEnumerable<BusinessEntityModule> GetBusinessEntityModules()
        {
            return GetCachedBusinessEntityModules().Values;
        }

        public BusinessEntityModule GetBusinessEntityModuleById(int moduleId)
        {
            var cachedModules = GetCachedBusinessEntityModules();
            return cachedModules.FindRecord(module => module.ModuleId == moduleId);
        }

        public string GetBusinessEntityModuleName(int moduleId)
        {
            BusinessEntityModule module = GetBusinessEntityModuleById(moduleId);
            return module != null ? module.Name : null;
        }

        public void SetCacheExpired()
        {
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        
        #endregion
        
        #region Private Methods

        Dictionary<int, BusinessEntityModule> GetCachedBusinessEntityModules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetModules",
            () =>
            {
                IBusinessEntityModuleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
                IEnumerable<BusinessEntityModule> modules = dataManager.GetModules();
                return modules.ToDictionary(module => module.ModuleId, module => module);
            });
        }
        
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBusinessEntityModuleDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreBusinessEntityModulesUpdated(ref _updateHandle);
            }
        }
        
        #endregion
    }
}
