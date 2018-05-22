using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;
namespace Vanrise.Common.Business
{
    public class EntityPersonalizationManager
    {
        public EntityPersonalizationData GetCurrentUserEntityPersonalization(List<string> entityUniqueNames)
        {
            EntityPersonalizationData entityPersonalization = new EntityPersonalizationData();

            List<EntityPersonalizationDetail> userPersonalizationItemSettings = new List<EntityPersonalizationDetail>();
            List<EntityPersonalizationDetail> globalPersonalizationItemSettings = new List<EntityPersonalizationDetail>();

            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            var entityPersonalizationsDictionaryByUser = GetCachedEntityPersonalizationsByUser().GetRecord(userId);
            foreach (var entityUniqueName in entityUniqueNames)
            {
                var userEntityPersonalization = entityPersonalizationsDictionaryByUser.GetRecord(entityUniqueName);
                var globalEntityPersonalization = GetGlobalEntityPersonalization(entityUniqueName);
                if (userEntityPersonalization != null && userEntityPersonalization.Setting != null)
                {
                    userPersonalizationItemSettings.Add(new EntityPersonalizationDetail()
                    {
                        EntityUniqueName = entityUniqueName,
                        ExtendedSetting = userEntityPersonalization.Setting
                    });
                }

                if (globalEntityPersonalization != null && globalEntityPersonalization.Setting != null)
                {
                    var globalEntityItemSettings = new EntityPersonalizationDetail()
                    {
                        EntityUniqueName = entityUniqueName,
                        ExtendedSetting = globalEntityPersonalization.Setting,
                        IsGlobal = true
                    };
                    if (userEntityPersonalization == null)
                        userPersonalizationItemSettings.Add(globalEntityItemSettings);
                    globalPersonalizationItemSettings.Add(globalEntityItemSettings);
                }
            }

            entityPersonalization.UserEntityPersonalizations = userPersonalizationItemSettings;
            entityPersonalization.GlobalEntityPersonalizations = globalPersonalizationItemSettings;

            return entityPersonalization;
        }
        public EntityPersonalization GetGlobalEntityPersonalization(string entityUniqueKey)
        {
            return GetCachedGlobalEntityPersonalizations().GetRecord(entityUniqueKey);
        }

        public void UpdateCurrentUserEntityPersonalization(List<EntityPersonalizationInput> inputs)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            foreach (var input in inputs)
            {
                EntityPersonalization entity = new EntityPersonalization()
                {
                    UserId = userId,
                    EntityUniqueName = input.EntityUniqueName,
                    Setting = input.ExtendedSetting,
                    CreatedBy = userId
                };
                SaveEntityPersonalization(entity);
            }
        }



        public void UpdateGlobalEntityPersonalization(List<EntityPersonalizationInput> inputs)
        {
            foreach (var input in inputs)
            {
                EntityPersonalization entity = new EntityPersonalization()
                {
                    EntityUniqueName = input.EntityUniqueName,
                    Setting = input.ExtendedSetting,
                    CreatedBy = ContextFactory.GetContext().GetLoggedInUserId()
                };
                SaveEntityPersonalization(entity);
            }
        }

        public void DeleteCurrentUserEntityPersonalization(List<string> entityUniqueNames)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            var entityPersonalizationsDictionaryByUser = GetCachedEntityPersonalizationsByUser().GetRecord(userId);

            foreach (var entityUniqueName in entityUniqueNames)
            {
                var userEntityPersonalizationUniqueKey = entityPersonalizationsDictionaryByUser.GetRecord(entityUniqueName);
                if (userEntityPersonalizationUniqueKey != null )
                {
                    long entityPersonalizationId = userEntityPersonalizationUniqueKey.EntityPersonalizationId;
                    Delete(entityPersonalizationId);
                }
            }
        }

        public void DeleteGlobalEntityPersonalization(List<string> entityUniqueNames)
        {
            foreach (var entityUniqueName in entityUniqueNames)
            {
                var entity = GetGlobalEntityPersonalization(entityUniqueName);
                if (entity != null)
                {
                    long entityPersonalizationId = entity.EntityPersonalizationId;
                    Delete(entityPersonalizationId);
                }
            }
           
        }

        void SaveEntityPersonalization(EntityPersonalization entity)
        {
            IEntityPersonalizationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IEntityPersonalizationDataManager>();
            dataManager.Save(entity);
        }

        void Delete(long entityPersonalizationId)
        {
            IEntityPersonalizationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IEntityPersonalizationDataManager>();
            bool deleteActionSucc = dataManager.Delete(entityPersonalizationId);

            if (deleteActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            
        }

        public Dictionary<string, EntityPersonalization> GetCachedGlobalEntityPersonalizations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedGlobalEntityPersonalizations",
                () =>
                {
                    var allGlobalEntities = GetCachedEntityPersonalizations().FindAllRecords(x => !x.UserId.HasValue);
                    return allGlobalEntities.ToDictionary(e => e.EntityUniqueName, e => e);
                });

        }

        public Dictionary<int, Dictionary<string, EntityPersonalization>> GetCachedEntityPersonalizationsByUser()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedEntityPersonalizationsByUser",
                () =>
                {
                    var allUserEntities = GetCachedEntityPersonalizations().FindAllRecords(x => x.UserId.HasValue);
                    var structuredUserEntityPersonalizations = new Dictionary<int, Dictionary<string, EntityPersonalization>>();
                    foreach (var e in allUserEntities)
                    {
                        var userEntity = structuredUserEntityPersonalizations.GetOrCreateItem(e.UserId.Value);
                        userEntity.Add(e.EntityUniqueName, e);
                    }
                    return structuredUserEntityPersonalizations;
                });

        }

        public List<EntityPersonalization> GetCachedEntityPersonalizations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetEntityPersonalizations",
               () =>
               {
                   IEntityPersonalizationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IEntityPersonalizationDataManager>();
                   return dataManager.GetEntityPersonalizations();
               });
        }


        #region private class
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IEntityPersonalizationDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IEntityPersonalizationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreEntityPersonalizationUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}
