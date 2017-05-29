using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Common;
using System.Data;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Data.SQL
{
    public class VRAlertRuleTypeDataManager : BaseSQLDataManager, IVRAlertRuleTypeDataManager
    {
        #region ctor/Local Variables
        public VRAlertRuleTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion


        #region Public Methods

        public List<VRAlertRuleType> GetVRAlertRuleTypes()
        {
            return GetItemsSP("VRNotification.sp_VRAlertRuleType_GetAll", VRAlertRuleTypeMapper);
        }

        public bool AreVRAlertRuleTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VRNotification.VRAlertRuleType", ref updateHandle);
        }

        public bool Insert(VRAlertRuleType vrAlertRuleTypeItem)
        {
            string serializedSettings = vrAlertRuleTypeItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrAlertRuleTypeItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertRuleType_Insert", vrAlertRuleTypeItem.VRAlertRuleTypeId, vrAlertRuleTypeItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(VRAlertRuleType vrAlertRuleTypeItem)
        {
            string serializedSettings = vrAlertRuleTypeItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrAlertRuleTypeItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("VRNotification.sp_VRAlertRuleType_Update", vrAlertRuleTypeItem.VRAlertRuleTypeId, vrAlertRuleTypeItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        public void GenerateScript(List<VRAlertRuleType> ruleTypes, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var ruleType in ruleTypes)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", ruleType.VRAlertRuleTypeId, ruleType.Name, Serializer.Serialize(ruleType.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[VRNotification].[VRAlertRuleType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);", scriptBuilder);
            addEntityScript("[VRNotification].[VRAlertRuleType]", script);
        }

        #endregion


        #region Mappers

        VRAlertRuleType VRAlertRuleTypeMapper(IDataReader reader)
        {
            VRAlertRuleType vrAlertRuleType = new VRAlertRuleType
            {
                VRAlertRuleTypeId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRAlertRuleTypeSettings>(reader["Settings"] as string) 
            };
            return vrAlertRuleType;
        }

        #endregion
    }
}
