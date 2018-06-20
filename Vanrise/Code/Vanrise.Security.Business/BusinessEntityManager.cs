using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class BusinessEntityManager
    {
        #region Fields / Constructors

        BusinessEntityModuleManager _beModuleManager;

        public BusinessEntityManager()
        {
            _beModuleManager = new BusinessEntityModuleManager();
        }

        #endregion

        #region Public Methods

        public IEnumerable<BusinessEntity> GetBusinessEntites()
        {
            return GetCachedBusinessEntities().Values;
        }

        public Vanrise.Entities.IDataRetrievalResult<BusinessEntityDetail> GetFilteredBusinessEntities(Vanrise.Entities.DataRetrievalInput<BusinessEntityQuery> input)
        {
            var businessEntites = GetBusinessEntites();

            Func<BusinessEntity, bool> filterExpression = (prod) =>
                 (prod.ModuleId == input.Query.ModuleId);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, businessEntites.ToBigResult(input, filterExpression, BusinessEntityDetailMapper));
        }

        public IEnumerable<BusinessEntityInfo> GetBusinessEntitiesByIds(List<Guid> entitiesIds)
        {
            var businessEntites = GetBusinessEntites();
            return businessEntites.MapRecords(BusinessEntityInfoMapper, (prod) => (entitiesIds.Contains(prod.EntityId)));
        }


        public BusinessEntity GetBusinessEntityById(Guid entityId)
        {
            var cachedEntities = GetCachedBusinessEntities();
            return cachedEntities.FindRecord(entity => entity.EntityId == entityId);
        }

        public IEnumerable<BusinessEntity> GetBusinessEntitiesByModuleId(Guid moduleId)
        {
            //TODO: pass the holder id to load the saved permissions
              var cachedEntities = GetCachedBusinessEntities();
              return cachedEntities.FindAllRecords(entity => entity.ModuleId == moduleId);
        }
        public string GetBusinessEntityName(EntityType entityType, string entityId)
        {
            Guid convertedEntityId = Guid.Parse(entityId);

            if (entityType == EntityType.MODULE)
                return _beModuleManager.GetBusinessEntityModuleName(convertedEntityId);

            BusinessEntity entity = GetBusinessEntityById(convertedEntityId);
            return entity != null ? entity.Name : null;
        }

        public void SetCacheExpired()
        {
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }
        public Vanrise.Entities.InsertOperationOutput<BusinessEntityDetail> AddBusinessEntity(BusinessEntity businessEntity)
        {
            InsertOperationOutput<BusinessEntityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BusinessEntityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            businessEntity.EntityId = Guid.NewGuid();

            IBusinessEntityDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            bool insertActionSucc = dataManager.AddBusinessEntity(businessEntity);

            if (insertActionSucc)
            {
                SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = BusinessEntityDetailMapper(businessEntity);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<BusinessEntityDetail> UpdateBusinessEntity(BusinessEntity businessEntity)
        {
            IBusinessEntityDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            bool updateActionSucc = dataManager.UpdateBusinessEntity(businessEntity);
            UpdateOperationOutput<BusinessEntityDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<BusinessEntityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BusinessEntityDetailMapper(businessEntity);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public bool UpdateBusinessEntityRank(Guid entityId, Guid moduleId)
        {
            IBusinessEntityDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
            return dataManager.UpdateBusinessEntityRank(entityId, moduleId);
        }
        #endregion

        #region Private Methods

        Dictionary<Guid, BusinessEntity> GetCachedBusinessEntities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBusinessEntites",
            () =>
            {
                IBusinessEntityDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IBusinessEntityDataManager>();
                IEnumerable<BusinessEntity> entities = dataManager.GetEntities();
                return entities.ToDictionary(entity => entity.EntityId, entity => entity);
            });
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

        #region Mapper
        BusinessEntityDetail BusinessEntityDetailMapper(BusinessEntity businessEntity)
        {
            return new BusinessEntityDetail
            {
                Entity = businessEntity
            };
        }

        BusinessEntityInfo BusinessEntityInfoMapper(BusinessEntity businessEntity)
        {
            return new BusinessEntityInfo
            {
                EntityId = businessEntity.EntityId,
                Name = businessEntity.Name
            };
        }
        #endregion
    }
}
