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
        List<SaleZone> GetSaleZones(int sellingNumberPlanId);

        void ApplySaleZonesForDB(object preparedSaleZones);

        void DeleteSaleZones(List<SaleZone> saleZones);

        void InsertSaleZones(List<SaleZone> saleZones);

        object InitialiazeStreamForDBApply();

        void WriteRecordToStream(SaleZone record, object dbApplyStream);

        object FinishDBApplyStream(object dbApplyStream);

        List<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter);

        bool AreZonesUpdated(ref object updateHandle);

        IEnumerable<SaleZone> GetAllSaleZones();
    }
}
