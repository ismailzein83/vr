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
                        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                        s_loggableEntityIds.Add(loggableEntity.EntityUniqueName, id);
                    }
                }
            }
            return id;
        }

        public string GenerateLoggableEntitiesScript(List<VRLoggableEntityBase> loggableEntitiesBehaviors, out string scriptEntityName)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            HashSet<Guid> addedloggableEntityIds = new HashSet<Guid>();
            foreach(var loggableEntityBehavior in loggableEntitiesBehaviors)
            {                
                Guid loggableEntityId = GetLoggableEntityId(loggableEntityBehavior);
                if (addedloggableEntityIds.Contains(loggableEntityId))
                    continue;
                VRLoggableEntity loggableEntity = GetCachedvrLoggableEntities().GetRecord(loggableEntityId);
                loggableEntity.ThrowIfNull("loggableEntityId", loggableEntityId);
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", loggableEntityId, loggableEntity.UniqueName, Serializer.Serialize(loggableEntity.Settings));
                addedloggableEntityIds.Add(loggableEntityId);
            }
            string script = String.Format(@"set nocount on;;with cte_data([ID],[UniqueName],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////{0}--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[UniqueName],[Settings]))merge	[logging].[LoggableEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]when not matched by target then	insert([ID],[UniqueName],[Settings])	values(s.[ID],s.[UniqueName],s.[Settings]);", scriptBuilder);
            scriptEntityName = "[logging].[LoggableEntity]";
            return script;
        }

        public Guid GetLoggableEntityId(string uniqueName)
        {
            var vrLoggableEntity = GetLoggableEntity(uniqueName);
            if (vrLoggableEntity == null)
                return Guid.Empty;
            return vrLoggableEntity.VRLoggableEntityId;
        }
        public VRLoggableEntity GetLoggableEntity(string uniqueName)
        {
            var vrLoggableEntities = GetCachedvrLoggableEntitiesByUniqueName();
            return vrLoggableEntities.GetRecord(uniqueName);
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
          private Dictionary<string, VRLoggableEntity> GetCachedvrLoggableEntitiesByUniqueName()
          {
              return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedLoggableEntitiesByUniqueName",
                () =>
                {
                    IVRLoggableEntityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLoggableEntityDataManager>();
                    IEnumerable<VRLoggableEntity> loggableEntities = dataManager.GetAll();
                    return loggableEntities.ToDictionary(a => a.UniqueName, a => a);
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
