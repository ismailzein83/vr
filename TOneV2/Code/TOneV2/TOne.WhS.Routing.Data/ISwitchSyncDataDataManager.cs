using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface ISwitchSyncDataDataManager : IDataManager, IRoutingDataManager
    {
        List<SwitchSyncData> GetSwitchSyncDataByIds(List<string> switchIds);

        void ApplySwitchesSyncData(List<string> switchIds, int versionNumber);
    }
}
