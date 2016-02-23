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

        public BusinessEntity GetBusinessEntityById(int entityId)
        {
            var cachedEntities = GetCachedBusinessEntities();
            return cachedEntities.FindRecord(entity => entity.EntityId == entityId);
        }

        public string GetBusinessEntityName(EntityType entityType, string entityId)
        {
            int convertedEntityId = Convert.ToInt32(entityId);

            if (entityType == EntityType.MODULE)
                return _beModuleManager.GetBusinessEntityModuleName(convertedEntityId);

            BusinessEntity entity = GetBusinessEntityById(convertedEntityId);
            return entity != null ? entity.Name : null;
        }

        public void SetCacheExpired()
        {
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        #endregion

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
