using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRLoggableEntityManager
    {
        internal Guid GetLoggableEntityId(VRLoggableEntityBase loggableEntity)
        {
            Guid id;
            if(!s_loggableEntityIds.TryGetValue(loggableEntity.EntityUniqueName, out id))
            {
                lock (s_loggableEntityIds)
                {
                    if (s_loggableEntityIds.Count == 0)
                        LoadAllLoggableEntities();
                    if (!s_loggableEntityIds.TryGetValue(loggableEntity.EntityUniqueName, out id))
                    {
                        IVRLoggableEntityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLoggableEntityDataManager>();
                        id = dataManager.AddOrUpdateLoggableEntity(loggableEntity.EntityUniqueName, new VRLoggableEntitySettings { ViewHistoryItemClientActionName = loggableEntity.ViewHistoryItemClientActionName });
                        s_loggableEntityIds.Add(loggableEntity.EntityUniqueName, id);
                    }
                }
            }
            return id;
        }

        public Guid GetLoggableEntityId(string uniqueName)
        {
            var vrLoggableEntity = GetLoggableEntity(uniqueName);
            if (vrLoggableEntity == null)
                throw new NullReferenceException(uniqueName);
            return vrLoggableEntity.VRLoggableEntityId;
        }
        public VRLoggableEntity GetLoggableEntity(string uniqueName)
        {
            var vrLoggableEntities = GetCachedvrLoggableEntities();
            return vrLoggableEntities.FindRecord(x=>x.UniqueName == uniqueName);
        }
          private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRLoggableEntityDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRLoggableEntityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRObjectTrackingUpdated(ref _updateHandle);
            }
        }



          private Dictionary<Guid, VRLoggableEntity> GetCachedvrLoggableEntities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedLoggableEntities",
              () =>
              {
                  IVRLoggableEntityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLoggableEntityDataManager>();
                  IEnumerable<VRLoggableEntity> loggableEntities = dataManager.GetAll();
                  return loggableEntities.ToDictionary(a => a.VRLoggableEntityId, a => a);
              });
        }

        private void LoadAllLoggableEntities()
        {
            IVRLoggableEntityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLoggableEntityDataManager>();
            List<VRLoggableEntity> allLoggableEntities = dataManager.GetAll();
            if (allLoggableEntities != null)
            {
                foreach (var loggableEntity in allLoggableEntities)
                {
                    s_loggableEntityIds.Add(loggableEntity.UniqueName, loggableEntity.VRLoggableEntityId);
                }
            }
        }

        static Dictionary<string, Guid> s_loggableEntityIds = new Dictionary<string, Guid>();
    }
}
