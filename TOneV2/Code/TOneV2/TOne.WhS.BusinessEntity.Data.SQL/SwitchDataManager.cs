using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SwitchDataManager : BaseSQLDataManager, ISwitchDataManager
    {

        #region ctor/Local Variables
        public SwitchDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public List<Switch> GetSwitches()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_Switch_GetAll]", SwitchMapper);
        }
        public bool Insert(Switch whsSwitch, out int insertedId)
        {
            object switchId;
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_Switch_Insert]", out switchId, whsSwitch.Name, Serializer.Serialize(whsSwitch.Settings), whsSwitch.CreatedBy, whsSwitch.LastModifiedBy);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)switchId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(SwitchToEdit whsSwitch)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_Switch_Update]", whsSwitch.SwitchId, whsSwitch.Name, Serializer.Serialize(whsSwitch.Settings), whsSwitch.LastModifiedBy);
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
        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        Switch SwitchMapper(IDataReader reader)
        {
            Switch whsSwitch = new Switch()
            {
                SwitchId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SourceId = reader["SourceID"] as string,
                Settings = Serializer.Deserialize<SwitchSettings>(reader["Settings"] as string),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime")
            };
            return whsSwitch;
        }
        #endregion

    }
}
