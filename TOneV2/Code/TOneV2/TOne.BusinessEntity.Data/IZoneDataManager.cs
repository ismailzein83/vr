﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IZoneDataManager : IDataManager
    {
        void LoadZonesInfo(DateTime effectiveTime, bool isFuture, List<CarrierAccountInfo> activeSuppliers, int batchSize, Action<List<ZoneInfo>> onBatchAvailable);
        //List<ZoneInfo> GetZones(string supplierId ,string nameFilter);
        List<ZoneInfo> GetSupplierZones(string supplierId, string nameFilter, DateTime whenDate);

        List<ZoneInfo> GetZoneList(IEnumerable<int> zonesIds);
        List<ZoneInfo> GetCustomerZones(string customerId, string nameFilter, DateTime whenDate);
        string GetZoneName(int zoneId);

        Dictionary<int, Zone> GetAllZones();

        //List<ZoneInfo> GetZonesBySupplierID(string supplierID);
    }
}
