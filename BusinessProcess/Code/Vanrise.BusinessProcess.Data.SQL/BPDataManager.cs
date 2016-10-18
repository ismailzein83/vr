using System;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Data.SQL
{
    internal class BPDataManager : BaseSQLDataManager, IBPDataManager
    {
        public BPDataManager()
            : base(GetConnectionStringName("BusinessProcessDBConnStringKey", "BusinessProcessDBConnString"))
        {
        }

        public T GetDefinitionObjectState<T>(Guid definitionId, string objectKey)
        {
            if (objectKey == null)
                objectKey = String.Empty;
            string objectVal = ExecuteScalarSP("bp.sp_BPDefinitionState_GetByKey", definitionId, objectKey) as string;
            return objectVal != null ? Serializer.Deserialize<T>(objectVal) : Activator.CreateInstance<T>();
        }

        public int InsertDefinitionObjectState(Guid definitionId, string objectKey, object objectValue)
        {
            if (objectKey == null)
                objectKey = String.Empty;
            return ExecuteNonQuerySP("bp.sp_BPDefinitionState_Insert", definitionId, objectKey, objectKey != null ? Serializer.Serialize(objectValue) : null);
        }

        public int UpdateDefinitionObjectState(Guid definitionId, string objectKey, object objectValue)
        {
            if (objectKey == null)
                objectKey = String.Empty;
            return ExecuteNonQuerySP("bp.sp_BPDefinitionState_Update", definitionId, objectKey, objectKey != null ? Serializer.Serialize(objectValue) : null);
        }
    }
}