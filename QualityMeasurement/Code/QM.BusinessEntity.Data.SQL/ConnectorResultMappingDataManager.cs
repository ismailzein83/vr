using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace QM.BusinessEntity.Data.SQL
{
    public class ConnectorResultMappingDataManager : BaseSQLDataManager, IConnectorResultMappingDataManager
    {
        public ConnectorResultMappingDataManager() :
            base(GetConnectionStringName("QM_BE_DBConnStringKey", "QM_BE_DBConnString"))
        {

        }

        public ConnectorResultMapping ConnectorResultMappingMapper(IDataReader reader)
        {
            ConnectorResultMapping connectorResultMapping = new ConnectorResultMapping()
            {
                ConnectorResultMappingId = (int)reader["ID"],
                ConnectorType = reader["ConnectorType"] as string,
                ResultId = (int)reader["ResultID"],
                ResultName = reader["ResultName"] as string,
                ConnectorResults = ((string)reader["ConnectorResults"]).Split('|').ToList()
            };
            return connectorResultMapping;
        }
        public bool AreConnectorResultMappingUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_BE.ConnectorResultMapping", ref updateHandle);
        }

        public List<ConnectorResultMapping> GetConnectorResultMappings()
        {
            return GetItemsSP("[QM_BE].[sp_ConnectorResultMapping_GetAll]", ConnectorResultMappingMapper);
        }
    }
}
