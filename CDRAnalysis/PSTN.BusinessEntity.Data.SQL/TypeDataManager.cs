using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class TypeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ITypeDataManager
    {
        public TypeDataManager() : base("CDRDBConnectionString") { }

        public List<Type> GetTypes()
        {
            return GetItemsSP("PSTN_BE.sp_SwitchType_GetAll", TypeMapper);
        }

        public Vanrise.Entities.BigResult<Type> GetFilteredTypes(Vanrise.Entities.DataRetrievalInput<TypeQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_CreateTempByName", tempTableName, input.Query.Name);
            }, (reader) => TypeMapper(reader));
        }

        public Type GetTypeById(int switchTypeId)
        {
            return GetItemSP("PSTN_BE.sp_SwitchType_GetByID", TypeMapper, switchTypeId);
        }

        public bool AddType(Type switchTypeObj, out int insertedId)
        {
            object switchTypeId;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Insert", out switchTypeId, switchTypeObj.Name);

            insertedId = (recordsAffected > 0) ? (int)switchTypeId : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateType(Type switchTypeObj)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Update", switchTypeObj.TypeId, switchTypeObj.Name);
            return (recordsAffected > 0);
        }

        public bool DeleteType(int switchTypeId)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchType_Delete", switchTypeId);
            return (recordsEffected > 0);
        }

        #region Mappers

        Type TypeMapper(IDataReader reader)
        {
            Type type = new Type();

            type.TypeId = (int)reader["ID"];
            type.Name = reader["Name"] as string;

            return type;
        }

        #endregion
    }
}
