using TOne.WhS.DBSync.Business;
using TOne.WhS.Routing.BP.Arguments;
using Vanrise.Entities;

namespace TOne.WhS.TOneV1Transition.Entities
{
    public class MigrationAndRouteBuildSettingsData : SettingData
    {
        public DBSyncTaskActionArgument DBSyncTaskActionArgument { get; set; }

        public RoutingProcessInput RoutingProcessInput { get; set; }
    }
}