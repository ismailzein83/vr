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
        public GenericBusinessEntityDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods
        public bool UpdateGenericBusinessEntity(Entities.GenericBusinessEntity genericBusinessEntity)
        {
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_GenericBusinessEntity_Update", genericBusinessEntity.GenericBusinessEntityId, genericBusinessEntity.BusinessEntityDefinitionId, Vanrise.Common.Serializer.Serialize(genericBusinessEntity.Details));
            return (recordesEffected > 0);
        }

        public bool AddGenericBusinessEntity(Entities.GenericBusinessEntity genericBusinessEntity, out long genericBusinessEntityId)
        {
            object insertedId;
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_GenericBusinessEntity_Insert", out insertedId, genericBusinessEntity.BusinessEntityDefinitionId, Vanrise.Common.Serializer.Serialize(genericBusinessEntity.Details));
            genericBusinessEntityId = (recordesEffected > 0) ? (long)insertedId : -1;
            return (recordesEffected > 0);
        }

        public List<Entities.GenericBusinessEntity> GetGenericBusinessEntitiesByDefinition(int businessDefinitionId)
        {
            return GetItemsSP("genericdata.sp_GenericBusinessEntity_GetByDefinition", GenericBusinessEntityMapper, businessDefinitionId);
        }

        public bool AreGenericBusinessEntityUpdated(int parameter,ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericBusinessEntity","BusinessEntityDefinitionID",parameter, ref updateHandle);
        }

        public bool DeleteGenericBusinessEntity(long genericBusinessEntityId, int businessEntityDefinitionId)
        {
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_GenericBusinessEntity_Delete", genericBusinessEntityId, businessEntityDefinitionId);
            return (affectedRows > 0);
        }
        #endregion

        #region Mappers
        GenericBusinessEntity GenericBusinessEntityMapper(IDataReader reader)
        {
            GenericBusinessEntity genericBusinessEntity = new GenericBusinessEntity
            {
                BusinessEntityDefinitionId = GetReaderValue<int>(reader, "BusinessEntityDefinitionID"),
                Details =Vanrise.Common.Serializer.Deserialize<dynamic>(reader["Details"] as string),
                GenericBusinessEntityId =(long)reader["ID"],
            };
            
            return genericBusinessEntity;
        }
        #endregion

    }
}
