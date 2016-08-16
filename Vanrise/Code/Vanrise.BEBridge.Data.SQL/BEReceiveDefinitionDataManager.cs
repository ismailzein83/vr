using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BEBridge.Data.SQL
{
    public class BEReceiveDefinitionDataManager : BaseSQLDataManager, IBEReceiveDefinitionDataManager
    {
        #region ctor/Local Variables
        public BEReceiveDefinitionDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnString", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region IBEReceiveDefinitionDataManager Implementation
        public IEnumerable<BEReceiveDefinition> GetBEReceiveDefinitions()
        {
            return GetItemsSP("[VR_BEBridge].[sp_BEReceiveDefinition_GetAll]", BEReceiveDefinitionMapper);
        }

        public bool AreBEReceiveDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[VR_BEBridge].[BEReceiveDefinition]", ref updateHandle);
        }

        #endregion

        #region Mappers
        BEReceiveDefinition BEReceiveDefinitionMapper(IDataReader reader)
        {
            return new BEReceiveDefinition
            {
                BEReceiveDefinitionId = GetReaderValue<Guid>(reader, "ID"),
                Name = reader["Name"] as string,
                Settings = reader["Settings"] != DBNull.Value ? Serializer.Deserialize<BEReceiveDefinitionSettings>(reader["Settings"] as string) : null
            };
        }

        #endregion

    }
}
