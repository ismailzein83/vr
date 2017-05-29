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
    class VRMailMessageTypeDataManager : BaseSQLDataManager, IVRMailMessageTypeDataManager
    {
        #region ctor/Local Variables
        public VRMailMessageTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #endregion


        #region Public Methods

        public List<VRMailMessageType> GetMailMessageTypes()
        {
            return GetItemsSP("common.sp_MailMessageType_GetAll", VRMailMessageTypeMapper);
        }

        public bool AreMailMessageTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.MailMessageType", ref updateHandle);
        }

        public bool Insert(VRMailMessageType vrMailMessageTypeItem)
        {
            string serializedSettings = vrMailMessageTypeItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrMailMessageTypeItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_MailMessageType_Insert", vrMailMessageTypeItem.VRMailMessageTypeId, vrMailMessageTypeItem.Name, serializedSettings);

            if (affectedRecords > 0)
            {
                return true;
            }

            return false;
        }

        public bool Update(VRMailMessageType vrMailMessageTypeItem)
        {
            string serializedSettings = vrMailMessageTypeItem.Settings != null ? Vanrise.Common.Serializer.Serialize(vrMailMessageTypeItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("common.sp_MailMessageType_Update", vrMailMessageTypeItem.VRMailMessageTypeId, vrMailMessageTypeItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        public void GenerateScript(List<VRMailMessageType> mailTypes, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var mailType in mailTypes)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", mailType.VRMailMessageTypeId, mailType.Name, Serializer.Serialize(mailType.Settings));
            }
            string script = String.Format(@"set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////{0}--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[MailMessageType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);", scriptBuilder);
            addEntityScript("[common].[MailMessageType]", script);
        }

        #endregion


        #region Mappers

        VRMailMessageType VRMailMessageTypeMapper(IDataReader reader)
        {
            VRMailMessageType vrMailMessageType = new VRMailMessageType
            {
                VRMailMessageTypeId = (Guid) reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRMailMessageTypeSettings>(reader["Settings"] as string) 
            };
            return vrMailMessageType;
        }

        #endregion
    }
}
