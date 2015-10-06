using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleZoneDataManager : IDataManager
    {
        List<SaleZone> GetSaleZones(int packageId);
        List<SaleZone> GetSaleZones(int packageId,DateTime effectiveDate);
        void ApplySaleZonesForDB(object preparedSaleZones);
        void DeleteSaleZones(List<SaleZone> saleZones);
        void InsertSaleZones(List<SaleZone> saleZones);
        object InitialiazeStreamForDBApply();
        void WriteRecordToStream(SaleZone record, object dbApplyStream);
        object FinishDBApplyStream(object dbApplyStream);

        List<SaleZoneInfo> GetSaleZonesInfo(int packageId, string filter);

        List<SaleZoneInfo> GetSaleZonesInfoByIds(int packageId, List<long> saleZoneIds);


        bool AreZonesUpdated(ref object updateHandle);
    }
}
