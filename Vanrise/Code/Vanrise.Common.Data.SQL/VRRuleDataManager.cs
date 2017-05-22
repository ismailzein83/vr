using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRRuleDataManager : BaseSQLDataManager, IVRRuleDataManager
    {
        #region ctor/Local Variables
        public VRRuleDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<VRRule> GetVRRules(Guid vrRuleDefinitionId)
        {
            return GetItemsSP("[common].[sp_VRRule_GetByDefinition]", VRRuleMapper, vrRuleDefinitionId);
        }

        public bool Insert(VRRule vrRule, out long vrRuleId)
        {
            object vrRuleID;
            string serializedSettings = vrRule.Settings != null ? Vanrise.Common.Serializer.Serialize(vrRule.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("[common].[sp_VRRule_Insert]", out vrRuleID, vrRule.VRRuleDefinitionId, serializedSettings);

            vrRuleId = (affectedRecords > 0) ? (long)vrRuleID : -1;
            return (affectedRecords > 0);
        }

        public bool Update(VRRule vrRule)
        {
            string serializedSettings = vrRule.Settings != null ? Vanrise.Common.Serializer.Serialize(vrRule.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[common].[sp_VRRule_Update]", vrRule.VRRuleId, vrRule.VRRuleDefinitionId, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreRulesUpdated(Guid vrRuleDefinitionId, ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[VRRule]", "RuleDefinitionId", vrRuleDefinitionId, ref updateHandle);
        }

        #endregion

        #region Mappers

        VRRule VRRuleMapper(IDataReader reader)
        {
            VRRule vrRule = new VRRule
            {
                VRRuleId = (long)reader["ID"],
                VRRuleDefinitionId = GetReaderValue<Guid>(reader, "RuleDefinitionId"),
                Settings = Vanrise.Common.Serializer.Deserialize<VRRuleSettings>(reader["Settings"] as string)
            };
            return vrRule;
        }

        #endregion
    }
}
