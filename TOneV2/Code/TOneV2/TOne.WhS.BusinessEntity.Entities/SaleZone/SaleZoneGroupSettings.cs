using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SaleZoneGroupSettings
    {
        
    }

    public class SelectiveSaleZonesSettings : SaleZoneGroupSettings
    {
        public List<long> ZoneIds { get; set; }
    }

    public class AllSaleZonesSettings : SaleZoneGroupSettings
    {
        
    }
}
