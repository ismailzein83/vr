using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProductSettings
    {
        public bool IsAllZones { get; set; }

        public short DefaultServicesFlag { get; set; }

        public List<RoutingProductZone> Zones { get; set; }

        public int? SaleZoneGroupConfigId { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public int? SupplierGroupConfigId { get; set; }

        public SupplierGroupSettings SupplierGroupSettings { get; set; }
    } 
   
    public class RoutingProductZone
    {
        public long ZoneId { get; set; }

        public short? ServicesFlag { get; set; }
    }
}
