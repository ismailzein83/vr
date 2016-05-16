using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Entities;

namespace QM.BusinessEntity.Data
{
    public interface IConnectorZoneInfoDataManager : IDataManager
    {
        bool UpdateZone(long connectorZoneInfoId, List<string> codes);
        bool AddZone(string connectorType, string connectorZoneId, List<string> codes);
        bool AreConnectorZonesInfoUpdated(ref object updateHandle);
        List<ConnectorZoneInfo> GetConnectorZonesInfo();
    }
}
