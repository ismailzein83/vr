using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOne.BusinessEntity.Entities;
using TOne.LCR.Entities;

namespace TOne.LCR.Data
{
    public interface IZoneRateDataManager : IDataManager, IRoutingDataManager
    {
        void InsertZoneRates(bool isSupplierZoneRates, List<ZoneRate> zoneRates);

        ZoneCustomerRates GetZoneCustomerRates(IEnumerable<Int32> lstZoneIds);

        SupplierZoneRates GetSupplierZoneRates(IEnumerable<Int32> lstZoneIds);

        CustomerSaleZones GetCustomerSaleZones(string customerId, string zoneName);

    }
}
