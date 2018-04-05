using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class RuntimeNodeConfigurationDataManager : BaseSQLDataManager, IRuntimeNodeConfigurationDataManager
    {
        public RuntimeNodeConfigurationDataManager()
            : base(GetConnectionStringName("RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString"))
        {

        }

        #region Public Methods

        public List<RuntimeNodeConfiguration> GetAllNodeConfigurations()
        {
            return GetItemsSP("[runtime].[sp_RuntimeNodeConfiguration_GetAll]", RuntimeNodeConfigurationMapper);
        }

        #endregion

        #region Private Methods

        private RuntimeNodeConfiguration RuntimeNodeConfigurationMapper(IDataReader reader)
        {
            var runtimeNodeConfiguration = new RuntimeNodeConfiguration
            {
                RuntimeNodeConfigurationId = (Guid)reader["ID"],
                Name = reader["Name"] as string
            };
            string serializedSettings = reader["Settings"] as string;
            if (serializedSettings != null)
                runtimeNodeConfiguration.Settings = Serializer.Deserialize<RuntimeNodeConfigurationSettings>(serializedSettings);
            return runtimeNodeConfiguration;
        }

        #endregion
    }
}
