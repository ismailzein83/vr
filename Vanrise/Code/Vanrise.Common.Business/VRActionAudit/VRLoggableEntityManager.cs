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
        internal int GetLoggableEntityId(VRLoggableEntityBase loggableEntity)
        {
            int id;
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

        static Dictionary<string, int> s_loggableEntityIds = new Dictionary<string, int>();
    }
}
