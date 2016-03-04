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
    public class GenericBusinessEntityDataManager:BaseSQLDataManager,IGenericBusinessEntityDataManager
    {
        public GenericBusinessEntityDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods
        public bool UpdateGenericBusinessEntity(Entities.GenericBusinessEntity genericBusinessEntity)
        {
            throw new NotImplementedException();
        }

        public bool AddGenericBusinessEntity(Entities.GenericBusinessEntity genericBusinessEntity, out long genericBusinessEntityId)
        {
            throw new NotImplementedException();
        }

        public List<Entities.GenericBusinessEntity> GetGenericBusinessEntities(int businessDefinitionId)
        {
            throw new NotImplementedException();
        }

        public bool AreGenericBusinessEntityUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericBusinessEntity", ref updateHandle);
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
