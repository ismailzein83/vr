using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using System.Data;

namespace Vanrise.Notification.Data.SQL
{
    public class VRAlertRuleDataManager : BaseSQLDataManager, IVRAlertRuleDataManager
    {
        #region ctor/Local Variables
        public VRAlertRuleDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<VRAlertRule> GetVRAlertRules()
        {
            return GetItemsSP("VRNotification.sp_VRAlertRule_GetAll", VRAlertRuleMapper);
        }

        public bool AreVRAlertRuleUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VRNotification.VRAlertRule", ref updateHandle);
        }

        public bool Insert(VRAlertRule vrAlertRule, out long insertedId)
        {
            object vrAlertRuleID;
            string serializedSettings = vrAlertRule.Settings != null ? Vanrise.Common.Serializer.Serialize(vrAlertRule.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertRule_Insert", out vrAlertRuleID, vrAlertRule.Name, vrAlertRule.RuleTypeId, vrAlertRule.UserId, vrAlertRule.CreatedBy, vrAlertRule.LastModifiedBy, serializedSettings);

            insertedId = (affectedRecords > 0) ? Convert.ToInt64(vrAlertRuleID) : -1;
            return (affectedRecords > 0);
        }

        public bool Update(VRAlertRule vrAlertRule)
        {
            string serializedSettings = vrAlertRule.Settings != null ? Vanrise.Common.Serializer.Serialize(vrAlertRule.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertRule_Update", vrAlertRule.VRAlertRuleId, vrAlertRule.Name, vrAlertRule.RuleTypeId, vrAlertRule.LastModifiedBy, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        VRAlertRule VRAlertRuleMapper(IDataReader reader)
        {
            VRAlertRule vrAlertRule = new VRAlertRule
            {
                VRAlertRuleId = (long) reader["ID"],
                Name = reader["Name"] as string,
                RuleTypeId = (Guid) reader["RuleTypeId"],
                UserId = (int) reader["UserID"],
                Settings = Vanrise.Common.Serializer.Deserialize<VRAlertRuleSettings>(reader["Settings"] as string) ,
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                CreatedBy = GetReaderValue<int?>(reader, "CreatedBy"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime"),
                LastModifiedBy = GetReaderValue<int?>(reader, "LastModifiedBy")
            };
            return vrAlertRule;
        }

        #endregion
    }
}
