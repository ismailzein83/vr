using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class BusinessEntityManager
    {
        BusinessEntityModuleManager _businessEntityModuleManager;

        public BusinessEntityManager()
        {
            _businessEntityModuleManager = new BusinessEntityModuleManager();
        }

        public List<BusinessEntityNode> GetEntityNodes()
        {
            //TODO: pass the holder id to load the saved permissions
            IBusinessEntityModuleDataManager moduleDataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
            IEnumerable<BusinessEntityModule> modules = moduleDataManager.GetModules();
            IEnumerable<BusinessEntity> entities = GetCachedBusinessEntities().Values;

            List<BusinessEntityNode> retVal = new List<BusinessEntityNode>();

            foreach (BusinessEntityModule item in modules)
            {
                if (item.ParentId == 0)
                {
                    retVal.Add(GetModuleNode(item, modules, entities, null));
                }
            }

            return retVal;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ToggleBreakInheritance(EntityType entityType, string entityId)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();
            updateOperationOutput.UpdatedObject = null;

            bool result = false;

            if (entityType == EntityType.MODULE)
            {
                IBusinessEntityModuleDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityModuleDataManager>();
                result = manager.ToggleBreakInheritance(entityId);
            }
            else
            {
                IBusinessEntityDataManager manager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
                result = manager.ToggleBreakInheritance(entityId);
            }

            updateOperationOutput.Result = (result) ? UpdateOperationResult.Succeeded : UpdateOperationResult.Failed;

            return updateOperationOutput;
        }

        public BusinessEntity GetBusinessEntityById(int entityId)
        {
            var cachedEntities = GetCachedBusinessEntities();
            BusinessEntity entity;
            cachedEntities.TryGetValue(entityId, out entity);
            return entity;
        }

        public string GetBusinessEntityName(EntityType entityType, string entityId)
        {
            int convertedEntityId = Convert.ToInt32(entityId);

            if (entityType == EntityType.MODULE)
                return _businessEntityModuleManager.GetBusinessEntityModuleName(convertedEntityId);

            BusinessEntity entity = GetBusinessEntityById(convertedEntityId);
            return entity != null ? entity.Name : null;
        }

        #region Private Methods

        Dictionary<int, BusinessEntity> GetCachedBusinessEntities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBusinessEntites",
            () =>
            {
                IBusinessEntityDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
                IEnumerable<BusinessEntity> entities = dataManager.GetEntities();
                return entities.ToDictionary(entity => entity.EntityId, entity => entity);
            });
        }

        private BusinessEntityNode GetModuleNode(BusinessEntityModule module, IEnumerable<BusinessEntityModule> modules, IEnumerable<BusinessEntity> entities, BusinessEntityNode parent)
        {
            BusinessEntityNode node = new BusinessEntityNode()
            {
                EntityId = module.ModuleId,
                Name = module.Name,
                EntType = EntityType.MODULE,
                BreakInheritance = module.BreakInheritance,
                PermissionOptions = module.PermissionOptions,
                Parent = parent
            };

            IEnumerable<BusinessEntityModule> subModules = modules.FindAllRecords(x => x.ParentId == module.ModuleId);

            IEnumerable<BusinessEntity> childEntities = entities.FindAllRecords(x => x.ModuleId == module.ModuleId);

            if (childEntities.Count() > 0)
            {
                node.Children = new List<BusinessEntityNode>();
                foreach (BusinessEntity entityItem in childEntities)
                {
                    BusinessEntityNode entityNode = new BusinessEntityNode()
                    {
                        EntityId = entityItem.EntityId,
                        Name = entityItem.Name,
                        EntType = EntityType.ENTITY,
                        BreakInheritance = entityItem.BreakInheritance,
                        PermissionOptions = entityItem.PermissionOptions,
                        Parent = node
                    };

                    node.Children.Add(entityNode);
                }
            }

            if (subModules.Count() > 0)
            {
                if (node.Children == null)
                    node.Children = new List<BusinessEntityNode>();

                foreach (BusinessEntityModule item in subModules)
                {
                    node.Children.Add(GetModuleNode(item, modules, entities, node));
                }
            }

            return node;
        }
        
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBusinessEntityDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreBusinessEntitiesUpdated(ref _updateHandle);
            }
        }
        
        #endregion
    }
}
