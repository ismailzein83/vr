using TOne.WhS.RouteSync.Data;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class SwitchCommandLogManager
    {
        public bool Insert(SwitchCommandLog switchCommandLog, out long insertedId)
        {
            ISwitchCommandLogDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<ISwitchCommandLogDataManager>();
            return dataManager.Insert(switchCommandLog, out insertedId);
        }
    }
}