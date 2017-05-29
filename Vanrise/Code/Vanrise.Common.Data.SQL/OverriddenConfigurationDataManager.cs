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
    public class OverriddenConfigurationDataManager : BaseSQLDataManager, IOverriddenConfigurationDataManager
    {
        #region ctor/Local Variables
      public OverriddenConfigurationDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnString", "ConfigurationDBConnString"))
        {

        }

        #endregion

      #region Public Methods
      public List<OverriddenConfiguration> GetOverriddenConfigurations()
      {
          return GetItemsSP("Common.sp_OverriddenConfiguration_GetAll", OverriddenConfigurationItemMapper);
      }
      public bool AreOverriddenConfigurationsUpdated(ref object updateHandle)
      {
          return base.IsDataUpdated("Common.OverriddenConfiguration", ref updateHandle);
      }
      public bool Insert(OverriddenConfiguration overriddenConfiguration)
      {
          string serializedSettings = overriddenConfiguration.Settings != null ? Vanrise.Common.Serializer.Serialize(overriddenConfiguration.Settings) : null;
          int affectedRecords = ExecuteNonQuerySP("Common.sp_OverriddenConfiguration_Insert", overriddenConfiguration.OverriddenConfigurationId, overriddenConfiguration.Name, overriddenConfiguration.GroupId, serializedSettings);
          return (affectedRecords > 0);

      }
      public bool Update(OverriddenConfiguration overriddenConfiguration)
      {
          string serializedSettings = overriddenConfiguration.Settings != null ? Vanrise.Common.Serializer.Serialize(overriddenConfiguration.Settings) : null;
          int affectedRecords = ExecuteNonQuerySP("Common.sp_OverriddenConfiguration_Update", overriddenConfiguration.OverriddenConfigurationId, overriddenConfiguration.Name, overriddenConfiguration.GroupId, serializedSettings);
          return (affectedRecords > 0);
      }

      public void GenerateScript(List<OverriddenConfiguration> overriddenConfigurations, Action<string, string> addEntityScript)
      {
          StringBuilder scriptBuilder = new StringBuilder();
          foreach (var itm in overriddenConfigurations)
          {
              if (scriptBuilder.Length > 0)
              {
                  scriptBuilder.Append(",");
                  scriptBuilder.AppendLine();
              }
              scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", itm.OverriddenConfigurationId, itm.Name, itm.GroupId, Serializer.Serialize(itm.Settings));
          }
          string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[GroupId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[GroupId],[Settings]))
merge	[common].[OverriddenConfiguration] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[GroupId] = s.[GroupId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[GroupId],[Settings])
	values(s.[ID],s.[Name],s.[GroupId],s.[Settings]);", scriptBuilder);
          addEntityScript("[common].[OverriddenConfiguration]", script);
      }

      #endregion

      #region Mappers
      OverriddenConfiguration OverriddenConfigurationItemMapper(IDataReader reader)
      {
          OverriddenConfiguration overriddenConfiguration = new OverriddenConfiguration
          {
              OverriddenConfigurationId = (Guid)reader["ID"],
              Name = reader["Name"] as string,
              GroupId = GetReaderValue<Guid>(reader, "GroupId"),
              Settings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<OverriddenConfigurationSettings>(reader["Settings"] as string) : null,
          };
          return overriddenConfiguration;
      }

      #endregion

    }
}
