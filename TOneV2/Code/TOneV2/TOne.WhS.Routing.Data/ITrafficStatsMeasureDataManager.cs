using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface ITrafficStatsMeasureDataManager : IDataManager
    {
        List<SupplierZoneTrafficStatsMeasure> GetQualityMeasurementsGroupBySupplierZone(TimeSpan timeSpan);
        List<SaleZoneSupplierTrafficStatsMeasure> GetQualityMeasurementsGroupBySaleZoneSupplier(TimeSpan timeSpan);
    }
}
