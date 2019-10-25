using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public int ZoneID { get; set; }

        public CustomerMapping CustomerMappings { get; set; }

        public SupplierMapping SupplierMappings { get; set; }
    }
}