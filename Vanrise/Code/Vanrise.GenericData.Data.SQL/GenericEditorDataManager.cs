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
    public class GenericEditorDataManager:BaseSQLDataManager,IGenericEditorDataManager
    {
        public GenericEditorDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
   
        #region Public Methods
        public List<GenericEditorDefinition> GetGenericEditorDefinitions()
        {
            return GetItemsSP("genericdata.sp_GenericEditorDefinition_GetAll", GenericEditorDefinitionMapper);
        }
        public bool AreGenericEditorDefinitionUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.GenericEditorDefinition", ref updateHandle);
        }
        public bool UpdateGenericEditorDefinition(GenericEditorDefinition genericEditorDefinition)
        {
            string serializedObj = null;
            if (genericEditorDefinition != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(genericEditorDefinition);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_GenericEditorDefinition_Update", genericEditorDefinition.GenericEditorDefinitionId, genericEditorDefinition.BusinessEntityId, serializedObj);
            return (recordesEffected > 0);
        }
        public bool AddGenericEditorDefinition(GenericEditorDefinition genericEditorDefinition, out int genericEditorDefinitionId)
        {
            object genericEditorDefinitionID;
            string serializedObj = null;
            if (genericEditorDefinition != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(genericEditorDefinition);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_GenericEditorDefinition_Insert", out genericEditorDefinitionID, genericEditorDefinition.BusinessEntityId, serializedObj);
            genericEditorDefinitionId = (recordesEffected > 0) ? (int)genericEditorDefinitionID : -1;

            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers
        GenericEditorDefinition GenericEditorDefinitionMapper(IDataReader reader)
        {
            GenericEditorDefinition genericEditorDefinition = Vanrise.Common.Serializer.Deserialize<GenericEditorDefinition>(reader["Details"] as string);
            if (genericEditorDefinition != null)
            {
                genericEditorDefinition.GenericEditorDefinitionId = Convert.ToInt32(reader["ID"]);
                genericEditorDefinition.BusinessEntityId = Convert.ToInt32(reader["BusinessEntityID"]);
            }
            return genericEditorDefinition;
        }

        #endregion
    }
}
