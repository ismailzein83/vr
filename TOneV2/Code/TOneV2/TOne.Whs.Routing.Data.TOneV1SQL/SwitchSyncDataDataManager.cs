using System.Collections.Generic;
using System.Data;
using System.Text;
using TOne.WhS.Routing.Entities;
using System.Linq;
using TOne.WhS.Routing.Data;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class SwitchSyncDataDataManager : RoutingDataManager, ISwitchSyncDataDataManager
    {
        public List<SwitchSyncData> GetSwitchSyncDataByIds(List<string> switchIds)
        {
            throw new System.NotImplementedException();
        }

        public void ApplySwitchesSyncData(List<string> switchIds, int versionNumber)
        {
          
        }
    }
}