using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Cataleya.Entities
{
    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<CustomerMapping> CustomerMappings { get; set; }

        public List<SupplierMapping> SupplierMappings { get; set; }
    }

    public class CustomerMapping
    {
        public string Mapping { get; set; }
    }

    public class SupplierMapping
    {
        public string Mapping { get; set; }
    }
}