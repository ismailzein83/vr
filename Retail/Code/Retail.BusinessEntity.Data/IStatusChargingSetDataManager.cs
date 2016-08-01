using System.Collections.Generic;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
    public interface IStatusChargingSetDataManager : IDataManager
    {
        List<StatusChargingSet> GetStatusChargingSets();
        bool Insert(StatusChargingSet statusDefinitionItem);
        bool Update(StatusChargingSet statusChargingSet);
    }
}
