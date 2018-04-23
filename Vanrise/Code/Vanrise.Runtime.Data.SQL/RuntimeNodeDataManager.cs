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
    public class RuntimeNodeDataManager : BaseSQLDataManager, IRuntimeNodeDataManager
    {
        public RuntimeNodeDataManager()
            : base(GetConnectionStringName("RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString"))
        {

        }

        #region Public Methods

        public List<RuntimeNode> GetAllNodes()
        {
            return GetItemsSP("[runtime].[sp_RuntimeNode_GetAll]", RuntimeNodeMapper);
        }

        public bool AreRuntimeNodeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[runtime].[RuntimeNode]", ref updateHandle);
        }
        #endregion

        #region Private Methods

        private RuntimeNode RuntimeNodeMapper(IDataReader reader)
        {
            var runtimeNode = new RuntimeNode
            {
                RuntimeNodeId = (Guid)reader["ID"],
                RuntimeNodeConfigurationId = (Guid)reader["RuntimeNodeConfigurationID"],
                Name = reader["Name"] as string
            };
            string serializedSettings = reader["Settings"] as string;
            if (serializedSettings != null)
                runtimeNode.Settings = Serializer.Deserialize<RuntimeNodeSettings>(serializedSettings);
            return runtimeNode;
        }

        #endregion
    }
}
