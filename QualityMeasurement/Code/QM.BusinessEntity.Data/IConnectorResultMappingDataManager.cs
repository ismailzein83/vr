using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Entities;

namespace QM.BusinessEntity.Data
{
    public interface IConnectorResultMappingDataManager : IDataManager
    {
        List<ConnectorResultMapping> GetConnectorResultMappings();
        bool AreConnectorResultMappingUpdated(ref object updateHandle);
    }
}
