using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Business;
using Vanrise.Entities;

namespace TOne.WhS.TOneV1Transition.Entities
{
    public class TOneV1TransitionSettingsData : SettingData
    {
        public DBSyncTaskActionArgument DBSyncTaskActionArgument { get; set; }
    }
}
