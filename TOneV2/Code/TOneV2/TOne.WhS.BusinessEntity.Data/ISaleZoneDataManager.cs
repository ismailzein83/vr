using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleZoneDataManager : IDataManager, IBulkApplyDataManager<SaleZone>
    {
        List<SaleZone> GetSaleZones(int packageId,DateTime effectiveDate);
        void ApplySaleZonesForDB(object preparedSaleZones);
        void DeleteSaleZones(List<SaleZone> saleZones);
    }
}
