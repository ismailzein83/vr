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
    public class ExtensionConfigurationDataManager : BaseSQLDataManager, IExtensionConfigurationDataManager
    {
        public ExtensionConfigurationDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }

        #region Public Methods
        public List<T> GetExtensionConfigurationsByType<T>(string type) where T : ExtensionConfiguration
        {
            return GetItemsSP("[Common].[SP_ExtensionConfiguration_GetByType]", ExtensionConfigurationMapper<T>, type);
        }

        public bool AreExtensionConfigurationUpdated(string parameter, ref object updateHandle)
        {
            return base.IsDataUpdated("Common.ExtensionConfiguration", "ConfigType", parameter, ref updateHandle);
        }
        #endregion

        #region Private Methods
      
        #endregion

        #region Mappers
        T ExtensionConfigurationMapper<T>(IDataReader reader) where T : ExtensionConfiguration
        {
            var extensionConfigurationObj = Vanrise.Common.Serializer.Deserialize<T>(reader["Settings"] as string);
            if (extensionConfigurationObj != null)
            {
                extensionConfigurationObj.ExtensionConfigurationId = (int)reader["OldID"];
                extensionConfigurationObj.Title = reader["Title"] as string;
                extensionConfigurationObj.Name = reader["Name"] as string;
            }
            return extensionConfigurationObj;
        }
        #endregion

    }
}
