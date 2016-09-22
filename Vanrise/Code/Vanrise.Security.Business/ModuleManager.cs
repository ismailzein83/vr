using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Caching;
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

        public bool UpdateModuleRank(Guid moduleId,Guid? parentId, int rank)
        {
            return _dataManager.UpdateModuleRank(moduleId,parentId, rank);
        }
        public Module GetModule(Guid moduleId)
        {
            var allModules = GetCachedModules();
            return allModules.GetRecord(moduleId);
        }

        public string GetModuleName(Guid moduleId)
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

        public Vanrise.Entities.InsertOperationOutput<ModuleDetail> AddModule(Module moduleObject)
        {
            InsertOperationOutput<ModuleDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ModuleDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            moduleObject.ModuleId =  Guid.NewGuid();
            IModuleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            bool insertActionSucc = dataManager.AddModule(moduleObject);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ModuleDetailMapper(moduleObject);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<ModuleDetail> UpdateModule(Module moduleObject)
        {
            IModuleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            bool updateActionSucc = dataManager.UpdateModule(moduleObject);
            UpdateOperationOutput<ModuleDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ModuleDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ModuleDetailMapper(moduleObject);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
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
        private Dictionary<Guid, Module> GetCachedModules()
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
        private ModuleDetail ModuleDetailMapper(Module moduleObject)
        {
            ModuleDetail moduleDetail = new ModuleDetail();
            moduleDetail.Entity = moduleObject;
            return moduleDetail;
        }

        #endregion
      
    }
}
