using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class BusinessEntityModuleManager
    {
        public BusinessEntityModule GetBusinessEntityModuleById(int moduleId)
        {
            var cachedModules = GetCachedBusinessEntities();
            BusinessEntityModule module;
            cachedModules.TryGetValue(moduleId, out module);
            return module;
        }

        public string GetBusinessEntityModuleName(int moduleId)
        {
            BusinessEntityModule module = GetBusinessEntityModuleById(moduleId);
            return module != null ? module.Name : null;
        }

        #region Private Methods

        Dictionary<int, BusinessEntityModule> GetCachedBusinessEntities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBusinessEntites",
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
