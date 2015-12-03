using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SwitchDataManager : BaseSQLDataManager, ISwitchDataManager
    {
        public SwitchDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }

        public Switch SwitchMapper(IDataReader reader)
        {
            Switch whsSwitch = new Switch()
            {
                SwitchId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            return whsSwitch;
        }

        public List<Switch> GetSwitches()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_Switch_GetAll]", SwitchMapper);
        }

        public bool Insert(Switch whsSwitch, out int insertedId)
        {
            object switchId;
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_Switch_Insert]", out switchId, whsSwitch.Name);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)switchId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }

        public bool Update(Switch whsSwitch)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_Switch_Update]",whsSwitch.SwitchId, whsSwitch.Name);
            return (recordsEffected > 0);
        }

        public bool AreSwitchesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.Switch", ref updateHandle);
        }


        public bool Delete(int switchId)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_Switch_Delete]", switchId);
            return (recordsEffected > 0);
        }
    }
}
