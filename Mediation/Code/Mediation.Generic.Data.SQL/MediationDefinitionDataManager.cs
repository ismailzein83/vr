using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;
using Vanrise.Data.SQL;

namespace Mediation.Generic.Data.SQL
{
    public class MediationDefinitionDataManager : BaseSQLDataManager, IMediationDefinitionDataManager
    {
        public MediationDefinitionDataManager()
            : base(GetConnectionStringName("Mediation_GenericConfig_DBConnStringKey", "Mediation_GenericConfig_DBConnString"))
        {

        }

        public List<MediationDefinition> GetMediationDefinitions()
        {
            return GetItemsSP("[Mediation_Generic].[sp_MediationDefinition_GetAll]", MediationDefinitionMapper);
        }

        public bool AreMediationDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Mediation_Generic.MediationDefinition", ref updateHandle);
        }

        public bool UpdateMediationDefinition(MediationDefinition mediationDefinition)
        {
            string serializedObj = null;
            serializedObj = Vanrise.Common.Serializer.Serialize(mediationDefinition);
            int recordesEffected = ExecuteNonQuerySP("Mediation_Generic.sp_MediationDefinition_Update", mediationDefinition.MediationDefinitionId, mediationDefinition.Name, serializedObj);
            return (recordesEffected > 0);
        }

        public bool AddMediationDefinition(MediationDefinition mediationDefinition, out int mediationDefinitionId)
        {
            object mediationDefinitionID;
            string serializedObj = null;
            serializedObj = Vanrise.Common.Serializer.Serialize(mediationDefinition);
            int recordesEffected = ExecuteNonQuerySP("Mediation_Generic.sp_MediationDefinition_Insert", out mediationDefinitionID, mediationDefinition.Name, serializedObj);
            mediationDefinitionId = (recordesEffected > 0) ? (int)mediationDefinitionID : -1;

            return (recordesEffected > 0);
        }

        #region Mappers
        private MediationDefinition MediationDefinitionMapper(IDataReader reader)
        {

            MediationDefinition mediationDefinition = new MediationDefinition();
            string details = reader["Details"] as string;
            if (details != null && details != string.Empty)
                mediationDefinition = Vanrise.Common.Serializer.Deserialize<MediationDefinition>(details);

            mediationDefinition.MediationDefinitionId = (int)reader["ID"];

            return mediationDefinition;
        }
        #endregion
    }
}
