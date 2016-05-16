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
    public class ConnectorZoneInfoDataManager : BaseSQLDataManager, IConnectorZoneInfoDataManager
    {
        public ConnectorZoneInfoDataManager() :
            base(GetConnectionStringName("QM_BE_DBConnStringKey", "QM_BE_DBConnString"))
        {

        }

        public ConnectorZoneInfo ConnectorZoneInfoMapper(IDataReader reader)
        {
            ConnectorZoneInfo connectorZoneInfo = new ConnectorZoneInfo()
            {
                ConnectorZoneInfoId = (int)reader["ID"],
                ConnectorType = reader["ConnectorType"] as string,
                ConnectorZoneId = reader["ConnectorZoneID"] as string,
                Codes = ((string)reader["Codes"]).Split(',').ToList()
            };
            return connectorZoneInfo;
        }

        public bool UpdateZone(long connectorZoneInfoId, List<string> codes)
        {
            int recordsEffected = ExecuteNonQuerySP("QM_BE.sp_ConnectorZoneInfo_Update", codes);
            return (recordsEffected > 0);
        }

        public bool AddZone(string connectorType, string connectorZoneId, List<string> codes)
        {
            ConnectorZoneInfo connectorZoneInfo = new ConnectorZoneInfo();
            connectorZoneInfo.Codes = codes;
            object connectorZoneInfoId;

            int recordsEffected = ExecuteNonQuerySP("QM_BE.sp_ConnectorZoneInfo_Insert", out connectorZoneInfoId, connectorType, connectorZoneId, string.Join(",", codes));
            return (recordsEffected > 0);
        }

        public List<ConnectorZoneInfo> GetConnectorZonesInfo()
        {
            return GetItemsSP("[QM_BE].[sp_ConnectorZoneInfo_GetAll]", ConnectorZoneInfoMapper);
        }

        public bool AreConnectorZonesInfoUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_BE.ConnectorZoneInfo", ref updateHandle);
        }
    }
}
