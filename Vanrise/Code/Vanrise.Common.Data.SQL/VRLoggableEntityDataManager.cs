using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRLoggableEntityDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IVRLoggableEntityDataManager
    {
        public VRLoggableEntityDataManager()
            : base(GetConnectionStringName("LoggingConfigDBConnStringKey", "LoggingConfigDBConnString"))
        {

        }
        public Guid AddOrUpdateLoggableEntity(string entityUniqueName, VRLoggableEntitySettings loggableEntitySettings)
        {
            return (Guid)ExecuteScalarSP("[logging].[sp_LoggableEntity_InsertOrUpdate]", entityUniqueName, Vanrise.Common.Serializer.Serialize(loggableEntitySettings));
        }

        public List<VRLoggableEntity> GetAll()
        {
            return GetItemsSP("[logging].[sp_LoggableEntity_GetAll]", 
                (reader) =>
                {
                    return new VRLoggableEntity
                    {
                        VRLoggableEntityId = (Guid)reader["ID"],
                        UniqueName = reader["UniqueName"] as string,
                        Settings = Vanrise.Common.Serializer.Deserialize<VRLoggableEntitySettings>(reader["Settings"] as string)
                    };
                });
        }

        public string GenerateScript(List<VRLoggableEntity> loggableEntities, out string scriptEntityName)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var loggableEntity in loggableEntities)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", loggableEntity.VRLoggableEntityId, loggableEntity.UniqueName, Serializer.Serialize(loggableEntity.Settings));
            }
            string script = String.Format(@"set nocount on;;with cte_data([ID],[UniqueName],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////{0}--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[UniqueName],[Settings]))merge	[logging].[LoggableEntity] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[UniqueName] = s.[UniqueName],[Settings] = s.[Settings]when not matched by target then	insert([ID],[UniqueName],[Settings])	values(s.[ID],s.[UniqueName],s.[Settings]);", scriptBuilder);
            scriptEntityName = "[logging].[LoggableEntity]";
            return script;
        }

        public bool AreVRObjectTrackingUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[logging].[LoggableEntity]", ref updateHandle);
        }
    }
}
