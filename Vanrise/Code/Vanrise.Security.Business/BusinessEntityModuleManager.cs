using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
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

        public BusinessEntityModule GetBusinessEntityModuleById(Guid moduleId)
        {
            var cachedModules = GetCachedBusinessEntityModules();
            return cachedModules.FindRecord(module => module.ModuleId == moduleId);
        }

        public string GetBusinessEntityModuleName(Guid moduleId)
        {
            BusinessEntityModule module = GetBusinessEntityModuleById(moduleId);
            return module != null ? module.Name : null;
        }
        public bool UpdateBusinessEntityModuleRank(Guid moduleId, Guid? parentId)
        {
            IBusinessEntityModuleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
            return dataManager.UpdateBusinessEntityModuleRank(moduleId, parentId);
        }
        public void SetCacheExpired()
        {
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityModule> AddBusinessEntityModule(BusinessEntityModule moduleObject)
        {
            InsertOperationOutput<BusinessEntityModule> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BusinessEntityModule>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            moduleObject.ModuleId = Guid.NewGuid();

            IBusinessEntityModuleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
            bool insertActionSucc = dataManager.AddBusinessEntityModule(moduleObject);

            if (insertActionSucc)
            {
                SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = moduleObject;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityModule> UpdateBusinessEntityModule(BusinessEntityModule moduleObject)
        {
            IBusinessEntityModuleDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
            bool updateActionSucc = dataManager.UpdateBusinessEntityModule(moduleObject);
            UpdateOperationOutput<BusinessEntityModule> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<BusinessEntityModule>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = moduleObject;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        #endregion
        
        #region Private Methods

        Dictionary<Guid, BusinessEntityModule> GetCachedBusinessEntityModules()
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
