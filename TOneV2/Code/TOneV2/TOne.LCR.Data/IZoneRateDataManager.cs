using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.LCR.Entities;

namespace TOne.LCR.Data
{
    public interface IZoneRateDataManager : IDataManager, IRoutingDataManager
    {
        void InsertZoneRates(bool isSupplierZoneRates, List<ZoneRate> zoneRates);
    }
}
