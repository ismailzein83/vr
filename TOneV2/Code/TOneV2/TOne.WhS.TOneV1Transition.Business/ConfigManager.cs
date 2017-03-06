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
            var tOneV1TransitionSettingsData = GetTOneV1TransitionSettingsData();

            if (tOneV1TransitionSettingsData.DBSyncTaskActionArgument == null)
                throw new NullReferenceException("tOneV1TransitionSettingsData.DBSyncTaskActionArgument");

            return tOneV1TransitionSettingsData.DBSyncTaskActionArgument;
        }

        public RoutingProcessInput GetRoutingProcessInput()
        {
            var tOneV1TransitionSettingsData = GetTOneV1TransitionSettingsData();

            if (tOneV1TransitionSettingsData.RoutingProcessInput == null)
                throw new NullReferenceException("tOneV1TransitionSettingsData.RoutingProcessInput");

            return tOneV1TransitionSettingsData.RoutingProcessInput;
        }

        #endregion

        #region private methods

        private TOneV1TransitionSettingsData GetTOneV1TransitionSettingsData()
        {
            SettingManager settingManager = new SettingManager();
            TOneV1TransitionSettingsData tOneV1TransitionSettingsData = settingManager.GetSetting<TOneV1TransitionSettingsData>(Constants.TOneV1TransitionSettingsData);

            if (tOneV1TransitionSettingsData == null)
                throw new NullReferenceException("tOneV1TransitionSettingsData");

            return tOneV1TransitionSettingsData;
        }

        #endregion
    }
}
