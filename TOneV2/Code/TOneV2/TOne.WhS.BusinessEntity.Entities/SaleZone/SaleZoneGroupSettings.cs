using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SaleZoneGroupSettings
    {
        public int? SaleZonePackageId { get; set; }
    }

    public class SelectiveSaleZonesSettings : SaleZoneGroupSettings
    {
        public List<long> ZoneIds { get; set; }
    }
}
