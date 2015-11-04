using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum RoutingProductZoneRelationType {  AllZones, SpecificZones}
    public enum RoutingProductSupplierRelationType { AllSuppliers, SpecificSuppliers}
    public class RoutingProductSettings
    {
        public short DefaultServicesFlag { get; set; }

        public RoutingProductZoneRelationType ZoneRelationType { get; set; }

        public List<RoutingProductZone> Zones { get; set; }

        public RoutingProductSupplierRelationType SupplierRelationType { get; set; }

        public List<RoutingProductSupplier> Suppliers { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public SupplierGroupSettings SupplierGroupSettings { get; set; }
    } 
   
    public class RoutingProductZone
    {
        public long ZoneId { get; set; }

        public short? ServicesFlag { get; set; }
    }

    public class RoutingProductSupplier
    {
        public int SupplierId { get; set; }
    }
}
