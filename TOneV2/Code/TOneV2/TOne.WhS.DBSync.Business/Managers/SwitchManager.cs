using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;


namespace TOne.WhS.DBSync.Business
{
    public class SwitchManager
    {

        public void AddSwitchesFromSource(List<Switch> switches)
        {
            SwitchDataManager dataManager = new SwitchDataManager("Switch");
            dataManager.ApplySwitchesToDB(switches);
        }
    }
}
