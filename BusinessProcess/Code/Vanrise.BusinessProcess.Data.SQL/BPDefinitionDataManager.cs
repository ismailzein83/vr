using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPDefinitionDataManager : BaseSQLDataManager, IBPDefinitionDataManager
    {
        public BPDefinitionDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }
        #region public methods
        public List<BPDefinition> GetBPDefinitions()
        {
            return GetItemsSP("bp.sp_BPDefinition_GetAll", BPDefinitionMapper);
        }

        public bool UpdateBPDefinition(BPDefinition bPDefinition)
        {
            int recordesEffected = ExecuteNonQuerySP("[bp].[sp_BPDefinition_Update]", bPDefinition.BPDefinitionID, bPDefinition.Title, Vanrise.Common.Serializer.Serialize(bPDefinition.Configuration));
            return (recordesEffected > 0);
        }

        public bool AreBPDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[BPDefinition]", ref updateHandle);
        }
        #endregion

        #region Mappers

        BPDefinition BPDefinitionMapper(IDataReader reader)
        {
            var bpDefinition = new BPDefinition
            {
                BPDefinitionID = (int)reader["ID"],
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                WorkflowType = Type.GetType(reader["FQTN"] as string)
            };
            string config = reader["Config"] as string;
            if (!String.IsNullOrWhiteSpace(config))
                bpDefinition.Configuration = Serializer.Deserialize<BPConfiguration>(config);
            return bpDefinition;
        }
        #endregion
    }
}
