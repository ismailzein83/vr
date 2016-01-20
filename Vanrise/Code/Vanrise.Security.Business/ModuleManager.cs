using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
namespace Vanrise.Security.Business
{
    public class ModuleManager
    {

        #region ctor

        private IModuleDataManager _dataManager;
        public ModuleManager()
        {
            _dataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
        }
        #endregion
      
        #region Public Members

        public bool UpdateModuleRank(int viewId, int rank)
        {
            return _dataManager.UpdateModuleRank(viewId, rank);
        }
        public Module GetModule(int moduleId)
        {
            var allModules = GetCachedModules();
            return allModules.GetRecord(moduleId);
        }

        public string GetModuleName(int moduleId)
        {
            Module module = GetModule(moduleId);
            if (module == null)
                return null;

            return module.Name;
        }
        public List<Module> GetModules()
        {
            return GetCachedModules().Values.ToList();
        }

        #endregion

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IModuleDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreModulesUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, Module> GetCachedModules()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetModules",
               () =>
               {
                   IModuleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
                   IEnumerable<Module> views = dataManager.GetModules();
                   return views.ToDictionary(kvp => kvp.ModuleId, kvp => kvp);
               });
        }

        #endregion

        #region  Mappers

        #endregion
      
    }
}
