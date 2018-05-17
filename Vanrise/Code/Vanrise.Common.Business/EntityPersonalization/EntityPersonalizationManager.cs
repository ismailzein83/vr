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
        public List<EntityPersonalizationDetail> GetCurrentUserEntityPersonalization(List<string> entityUniqueNames)
        {
            List<EntityPersonalizationDetail> entityPersonalizationItemSettings = new List<EntityPersonalizationDetail>();
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            var entityPersonalizationsDictionaryByUser = GetCachedEntityPersonalizationsByUser().GetRecord(userId);
            if (entityPersonalizationsDictionaryByUser == null || entityPersonalizationsDictionaryByUser.Count == 0)
                return null;
            foreach (var entityUniqueName in entityUniqueNames)
            {
                var userEntityPersonalizationUniqueKey = entityPersonalizationsDictionaryByUser.GetRecord(entityUniqueName);
                if (userEntityPersonalizationUniqueKey != null && userEntityPersonalizationUniqueKey.Setting != null)
                    entityPersonalizationItemSettings.Add(new EntityPersonalizationDetail()
                    {
                        EntityUniqueName = entityUniqueName,
                        ExtendedSetting = userEntityPersonalizationUniqueKey.Setting
                    });
            }

            return entityPersonalizationItemSettings;
        }
        public EntityPersonalizationExtendedSetting GetGlobalEntityPersonalization(string entityUniqueKey)
        {
            return GetCachedGlobalEntityPersonalizations().GetRecord(entityUniqueKey).Setting;
        }

        public void UpdateCurrentUserEntityPersonalization(List<EntityPersonalizationInput> inputs)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            IEntityPersonalizationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IEntityPersonalizationDataManager>();
            foreach (var input in inputs)
            {
                EntityPersonalization entity = new EntityPersonalization()
                {
                    UserId = userId,
                    EntityUniqueName = input.EntityUniqueName,
                    Setting = input.ExtendedSetting,
                    CreatedBy = userId
                };               
                dataManager.Save(entity);
            }

        }

        public void UpdateGlobalEntityPersonalization(List<EntityPersonalizationInput> inputs)
        {
            throw new NotImplementedException();
        }

        public void DeleteCurrentUserEntityPersonalization(string entityUniqueKey)
        {
            throw new NotImplementedException();
        }

        public void DeleteGlobalEntityPersonalization(string entityUniqueKey)
        {
            throw new NotImplementedException();
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
