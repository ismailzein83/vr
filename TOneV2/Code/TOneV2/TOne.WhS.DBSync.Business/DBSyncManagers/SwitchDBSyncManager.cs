using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;


namespace TOne.WhS.DBSync.Business
{
    public class SwitchDBSyncManager
    {

        public void ApplySwitchesToTemp(List<Switch> switches)
        {
            SwitchDBSyncDataManager dataManager = new SwitchDBSyncDataManager();
            dataManager.ApplySwitchesToTemp(switches);
        }
    }
}
