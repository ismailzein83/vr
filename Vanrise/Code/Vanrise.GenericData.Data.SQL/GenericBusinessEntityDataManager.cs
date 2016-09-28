using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class GenericBusinessEntityDataManager : BaseSQLDataManager, IGenericBusinessEntityDataManager
    {
        IGenericBusinessEntityManager _genericBusinessEntityManager = BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
        IDataRecordTypeManager _recordTypeManager = BusinessManagerFactory.GetManager<IDataRecordTypeManager>();
        Guid _dataRecordTypeId;

        public GenericBusinessEntityDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods
        public List<Entities.GenericBusinessEntity> GetGenericBusinessEntitiesByDefinition(int businessDefinitionId)
        {
            _dataRecordTypeId = _genericBusinessEntityManager.GetDataRecordTypeId(businessDefinitionId);
            return GetItemsSP("genericdata.sp_GenericBusinessEntity_GetByDefinition", GenericBusinessEntityMapper, businessDefinitionId);
        }
        public bool AddGenericBusinessEntity(Entities.GenericBusinessEntity genericBusinessEntity, out long genericBusinessEntityId)
        {
            object insertedId;
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_GenericBusinessEntity_Insert", out insertedId, genericBusinessEntity.BusinessEntityDefinitionId, GetSerializedDetails(genericBusinessEntity));
            genericBusinessEntityId = (recordesEffected > 0) ? (long)insertedId : -1;
            return (recordesEffected > 0);
        }
        public bool UpdateGenericBusinessEntity(Entities.GenericBusinessEntity genericBusinessEntity)
        {
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_GenericBusinessEntity_Update", genericBusinessEntity.GenericBusinessEntityId, genericBusinessEntity.BusinessEntityDefinitionId, GetSerializedDetails(genericBusinessEntity));
            return (recordesEffected > 0);
        }
        public bool DeleteGenericBusinessEntity(long genericBusinessEntityId, int businessEntityDefinitionId)
        {
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericBusinessEntity_Delete", genericBusinessEntityId, businessEntityDefinitionId);
            return (affectedRows > 0);
        }
        public bool AreGenericBusinessEntityUpdated(int parameter,ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericBusinessEntity","BusinessEntityDefinitionID",parameter, ref updateHandle);
        }
        #endregion

        #region Private Methods
        string GetSerializedDetails(GenericBusinessEntity genericBusinessEntity)
        {
            Guid dataRecordTypeId = _genericBusinessEntityManager.GetDataRecordTypeId(genericBusinessEntity.BusinessEntityDefinitionId);
            return _recordTypeManager.SerializeRecord(genericBusinessEntity.Details, dataRecordTypeId);
        }
        #endregion

        #region Mappers
        GenericBusinessEntity GenericBusinessEntityMapper(IDataReader reader)
        {
            return new GenericBusinessEntity
            {
                GenericBusinessEntityId = (long)reader["ID"],
                BusinessEntityDefinitionId = GetReaderValue<int>(reader, "BusinessEntityDefinitionID"),
                Details = _recordTypeManager.DeserializeRecord(reader["Details"] as string, _dataRecordTypeId)
            };
        }
        #endregion
    }
}
