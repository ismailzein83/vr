using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;


namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SwitchReleaseCauseDataManager : BaseSQLDataManager, ISwitchReleaseCauseDataManager
    {
        public SwitchReleaseCauseDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }

        public List<SwitchReleaseCause> GetSwitchReleaseCauses()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_SwitchReleaseCause_GetAll]", SwitchReleaseCauseMapper);
        }
        public bool AddSwitchReleaseCause(SwitchReleaseCause switchReleaseCause, out int insertedId)
        {
            object switchReleaseCauseId;
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_SwitchReleaseCause_Insert]", out switchReleaseCauseId, switchReleaseCause.SwitchId, switchReleaseCause.ReleaseCode, Serializer.Serialize(switchReleaseCause.Settings), switchReleaseCause.SourceId, switchReleaseCause.CreatedBy, switchReleaseCause.LastModifiedBy);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)switchReleaseCauseId;
            else
                insertedId = 0;
            return insertedSuccesfully;

        }
        public bool UpdateSwitchReleaseCause(SwitchReleaseCause switchReleaseCause)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_SwitchReleaseCause_Update]", switchReleaseCause.SwitchReleaseCauseId, switchReleaseCause.SwitchId, switchReleaseCause.ReleaseCode, Serializer.Serialize(switchReleaseCause.Settings), switchReleaseCause.SourceId, switchReleaseCause.LastModifiedBy);
            return (recordsEffected > 0);
        }
        SwitchReleaseCause SwitchReleaseCauseMapper(IDataReader reader)
        {
            return new SwitchReleaseCause()
            {
                ReleaseCode = reader["ReleaseCode"] as string,
                SwitchReleaseCauseId = (int)reader["ID"],
                SwitchId = (int)reader["SwitchID"],
                Settings = Serializer.Deserialize<SwitchReleaseCauseSetting>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime"),
            };
        }
        public bool AreSwitchReleaseCausesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SwitchReleaseCause", ref updateHandle);
        }
    }
}
