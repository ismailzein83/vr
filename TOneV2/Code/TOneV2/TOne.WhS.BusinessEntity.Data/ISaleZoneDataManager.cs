﻿using System;
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
        List<SaleZone> GetSaleZones(int sellingNumberPlanId,DateTime effectiveDate);
        void ApplySaleZonesForDB(object preparedSaleZones);
        void DeleteSaleZones(List<SaleZone> saleZones);
        void InsertSaleZones(List<SaleZone> saleZones);
        object InitialiazeStreamForDBApply();
        void WriteRecordToStream(SaleZone record, object dbApplyStream);
        object FinishDBApplyStream(object dbApplyStream);

        List<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter);

        List<SaleZoneInfo> GetSaleZonesInfoByIds(int sellingNumberPlanId, List<long> saleZoneIds);


        bool AreZonesUpdated(ref object updateHandle);

        IEnumerable<SaleZone> GetAllSaleZones();
    }
}
