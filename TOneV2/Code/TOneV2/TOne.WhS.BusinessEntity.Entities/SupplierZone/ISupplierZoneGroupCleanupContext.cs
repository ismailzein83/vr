using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SupplierZoneGroupCleanupResult { NoChange = 0, ZonesUpdated = 1, AllZonesRemoved = 2 };

    public interface ISupplierZoneGroupCleanupContext
    {
        SupplierZoneGroupCleanupResult Result { get; set; }

        IEnumerable<long> DeletedSupplierZoneIds { get; set; }
    }
}
