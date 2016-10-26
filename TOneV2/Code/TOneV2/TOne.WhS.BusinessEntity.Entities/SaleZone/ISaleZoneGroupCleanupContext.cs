using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SaleZoneGroupCleanupResult { NoChange = 0, ZonesUpdated = 1, AllZonesRemoved = 2};

    public interface ISaleZoneGroupCleanupContext
    {
        SaleZoneGroupCleanupResult Result { get; set; }

        IEnumerable<long> DeletedSaleZoneIds { get; set; }
    }
}
