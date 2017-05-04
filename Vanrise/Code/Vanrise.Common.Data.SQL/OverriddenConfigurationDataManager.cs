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
