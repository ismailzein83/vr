using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDataManager : BaseSQLDataManager, ISwitchDataManager
    {
        public SwitchDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }
        public void InsertSwitchFromSource(Switch whsSwitch)
        {
            object switchId;
            ExecuteNonQuerySP("[TOneWhS_BE].[sp_Switch_Insert]", out switchId, whsSwitch.Name);
        }

    }
}
