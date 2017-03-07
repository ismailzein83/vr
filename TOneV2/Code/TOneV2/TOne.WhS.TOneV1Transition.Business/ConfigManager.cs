using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Business;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.TOneV1Transition.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.TOneV1Transition.Business
{
    public class ConfigManager
    {
        #region public methods

        public DBSyncTaskActionArgument GetDBSyncTaskActionArgument()
        {
            var tOneV1TransitionSettingsData = GetMigrationAndRouteBuildSettingsData();

            if (tOneV1TransitionSettingsData.DBSyncTaskActionArgument == null)
                throw new NullReferenceException("tOneV1TransitionSettingsData.DBSyncTaskActionArgument");

            return tOneV1TransitionSettingsData.DBSyncTaskActionArgument;
        }

        public RoutingProcessInput GetRoutingProcessInput()
        {
            var tOneV1TransitionSettingsData = GetMigrationAndRouteBuildSettingsData();

            if (tOneV1TransitionSettingsData.RoutingProcessInput == null)
                throw new NullReferenceException("tOneV1TransitionSettingsData.RoutingProcessInput");

            return tOneV1TransitionSettingsData.RoutingProcessInput;
        }

        #endregion

        #region private methods

        private MigrationAndRouteBuildSettingsData GetMigrationAndRouteBuildSettingsData()
        {
            SettingManager settingManager = new SettingManager();
            MigrationAndRouteBuildSettingsData migrationAndRouteBuildSettingsData = settingManager.GetSetting<MigrationAndRouteBuildSettingsData>(Constants.MigrationAndRouteBuildSettingsData);

            if (migrationAndRouteBuildSettingsData == null)
                throw new NullReferenceException("migrationAndRouteBuildSettingsData");

            return migrationAndRouteBuildSettingsData;
        }

        #endregion
    }
}
