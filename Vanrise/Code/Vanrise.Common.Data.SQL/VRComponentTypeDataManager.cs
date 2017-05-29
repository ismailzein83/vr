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
    public class VRComponentTypeDataManager : BaseSQLDataManager, IVRComponentTypeDataManager
    {
        #region Ctor/Properties

        public VRComponentTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Mehods

        public List<VRComponentType> GetComponentTypes()
        {
            return GetItemsSP("[common].[sp_VRComponentType_GetAll]", VRComponentTypeMapper);
        }

        public bool Insert(VRComponentType componentType)
        {
            string serializedSettings = componentType.Settings != null ? Vanrise.Common.Serializer.Serialize(componentType.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[common].[sp_VRComponentType_Insert]", componentType.VRComponentTypeId, componentType.Name, componentType.Settings.VRComponentTypeConfigId, serializedSettings);

            if (affectedRecords > 0)
                return true;

            return false;
        }

        public bool Update(VRComponentType componentType)
        {
            string serializedSettings = componentType.Settings != null ? Vanrise.Common.Serializer.Serialize(componentType.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[common].[sp_VRComponentType_Update]", componentType.VRComponentTypeId, componentType.Name, componentType.Settings.VRComponentTypeConfigId, serializedSettings);
            return (affectedRecords > 0);
        }

        public bool AreVRComponentTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRComponentType", ref updateHandle);
        }

        public void GenerateScript(List<VRComponentType> componentTypes, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var componentType in componentTypes)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", componentType.VRComponentTypeId, componentType.Name, componentType.Settings.VRComponentTypeConfigId, Serializer.Serialize(componentType.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);", scriptBuilder);
            addEntityScript("[common].[VRComponentType]", script);
        }

        #endregion

        #region Mappers

        VRComponentType VRComponentTypeMapper(IDataReader reader)
        {
            return new VRComponentType
            {
                VRComponentTypeId = GetReaderValue<Guid>(reader, "ID"),
                Name = reader["Name"] as string,
                Settings = Serializer.Deserialize<VRComponentTypeSettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}
