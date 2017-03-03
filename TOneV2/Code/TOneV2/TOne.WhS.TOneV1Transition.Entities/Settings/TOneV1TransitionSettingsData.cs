using TOne.WhS.DBSync.Business;
using TOne.WhS.Routing.BP.Arguments;
using Vanrise.Entities;

namespace TOne.WhS.TOneV1Transition.Entities
{
    public class TOneV1TransitionSettingsData : SettingData
    {
        public DBSyncTaskActionArgument DBSyncTaskActionArgument { get; set; }

        public RoutingProcessInput RoutingProcessInput { get; set; }

        public Settings Settings { get; set; }
    }

    public class Settings
    {
        public int RoutingMigrationOffsetInMin { get; set; }
    }
}