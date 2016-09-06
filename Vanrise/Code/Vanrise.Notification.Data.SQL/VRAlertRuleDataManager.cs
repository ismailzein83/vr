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

        public bool Insert(VRAlertRule vrAlertRuleItem, out long insertedId)
        {
            object vrAlertRuleID;
            string serializedSettings = vrAlertRuleItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrAlertRuleItem.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertRule_Insert", out vrAlertRuleID, vrAlertRuleItem.Name, vrAlertRuleItem.RuleTypeId, serializedSettings);

            insertedId = (affectedRecords > 0) ? (long)vrAlertRuleID : -1;
            return (affectedRecords > 0);
        }

        public bool Update(VRAlertRule vrAlertRuleItem)
        {
            string serializedSettings = vrAlertRuleItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrAlertRuleItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertRule_Update", vrAlertRuleItem.VRAlertRuleId, vrAlertRuleItem.Name, vrAlertRuleItem.Settings, serializedSettings);
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
                //Settings = Vanrise.Common.Serializer.Deserialize<VRAlertRuleSettings>(reader["Settings"] as string) 
            };
            return vrAlertRule;
        }

        #endregion
    }
}
