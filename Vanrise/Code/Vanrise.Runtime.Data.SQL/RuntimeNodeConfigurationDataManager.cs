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

        public bool Insert(RuntimeNodeConfiguration nodeConfig)
        {

            string serializedSetting = nodeConfig.Settings != null ? Vanrise.Common.Serializer.Serialize(nodeConfig.Settings) : null;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[runtime].[sp_RuntimeNodeConfiguration_Insert]", nodeConfig.RuntimeNodeConfigurationId, nodeConfig.Name, serializedSetting);
            return (nbOfRecordsAffected > 0);
        }

        public bool Update(RuntimeNodeConfiguration nodeConfig)
        {
            string serializedSetting = nodeConfig.Settings != null ? Vanrise.Common.Serializer.Serialize(nodeConfig.Settings) : null;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[runtime].[sp_RuntimeNodeConfiguration_Update]", nodeConfig.RuntimeNodeConfigurationId, nodeConfig.Name, serializedSetting);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreRuntimeNodeConfigurationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[runtime].[RuntimeNodeConfiguration]", ref updateHandle);
        }

        #endregion

        #region Private Methods

        private RuntimeNodeConfiguration RuntimeNodeConfigurationMapper(IDataReader reader)
        {
            return new RuntimeNodeConfiguration
            {
                Name = reader["Name"] as string,
                RuntimeNodeConfigurationId = (Guid)reader["ID"],
                Settings = Serializer.Deserialize<RuntimeNodeConfigurationSettings>(reader["Settings"] as string)
 
            };
        }

        #endregion
    }
}
