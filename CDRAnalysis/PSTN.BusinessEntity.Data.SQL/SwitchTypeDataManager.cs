using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class SwitchTypeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISwitchTypeDataManager
    {
        public SwitchTypeDataManager() : base("CDRDBConnectionString") { }

        public Vanrise.Entities.BigResult<SwitchType> GetFilteredSwitchTypes(Vanrise.Entities.DataRetrievalInput<SwitchTypeQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_CreateTempByName", tempTableName, input.Query.Name);
            }, (reader) => SwitchTypeMapper(reader));
        }

        public SwitchType GetSwitchTypeByID(int switchTypeID)
        {
            return GetItemSP("PSTN_BE.sp_SwitchType_GetByID", SwitchTypeMapper, switchTypeID);
        }

        public bool AddSwitchType(SwitchType switchTypeObject, out int insertedID)
        {
            object switchTypeID;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Insert", out switchTypeID, switchTypeObject.Name);

            insertedID = (recordsAffected > 0) ? (int)switchTypeID : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateSwitchType(SwitchType switchTypeObject)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Update", switchTypeObject.ID, switchTypeObject.Name);
            return (recordsAffected > 0);
        }

        public bool DeleteSwitchType(int switchTypeID)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Delete", switchTypeID);
            return (recordsEffected > 0);
        }

        #region Mappers

        SwitchType SwitchTypeMapper(IDataReader reader)
        {
            SwitchType type = new SwitchType();

            type.ID = (int)reader["ID"];
            type.Name = reader["Name"] as string;

            return type;
        }

        #endregion
    }
}
