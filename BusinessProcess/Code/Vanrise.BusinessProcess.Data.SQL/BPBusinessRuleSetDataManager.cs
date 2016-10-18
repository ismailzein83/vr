using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using System;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPBusinessRuleSetDataManager : BaseSQLDataManager, IBPBusinessRuleSetDataManager
    {
        public BPBusinessRuleSetDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #region public methods
        public List<BPBusinessRuleSet> GetBPBusinessRuleSets()
        {
            return GetItemsSP("bp.sp_BPBusinessRuleSet_GetAll", BPBusinessRuleSetMapper);
        }

        public bool AddBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj, out int bpBusinessRuleSetId)
        {
            object insertedId;

            int recordesEffected = ExecuteNonQuerySP("bp.sp_BPBusinessRuleSet_Insert", out insertedId, businessRuleSetObj.Name, businessRuleSetObj.ParentId,
                Common.Serializer.Serialize(businessRuleSetObj.Details), businessRuleSetObj.BPDefinitionId);
            bpBusinessRuleSetId = recordesEffected > 0 ? (int)insertedId : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj)
        {
            int recordesEffected = ExecuteNonQuerySP("bp.sp_BPBusinessRuleSet_Update", businessRuleSetObj.BPBusinessRuleSetId, businessRuleSetObj.Name, businessRuleSetObj.ParentId,
                Common.Serializer.Serialize(businessRuleSetObj.Details), businessRuleSetObj.BPDefinitionId);
            return (recordesEffected > 0);
        }

        public bool AreBPBusinessRuleSetsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[BPBusinessRuleSet]", ref updateHandle);
        }
        #endregion

        #region Mappers

        BPBusinessRuleSet BPBusinessRuleSetMapper(IDataReader reader)
        {
            BPBusinessRuleSet BPBusinessRuleSet = new BPBusinessRuleSet()
            {
                BPBusinessRuleSetId = (int)reader["ID"],
                Name = reader["Name"] as string,
                ParentId = GetReaderValue<int?>(reader, "ParentID"),
                Details = Serializer.Deserialize<BPBusinessRuleSetDetails>(reader["Details"] as string),
                BPDefinitionId = GetReaderValue<Guid>(reader,"BPDefinitionId")
            };
            return BPBusinessRuleSet;
        }
        #endregion
    }
}