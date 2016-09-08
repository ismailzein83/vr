using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class RouteSyncTechnicalSettings : Vanrise.Entities.SettingData
    {
        public const string SETTING_TYPE = "WhS_RouteSync_TechnicalSettings";

        public SwitchInfoGetter SwitchInfoGetter { get; set; }
    }
}
