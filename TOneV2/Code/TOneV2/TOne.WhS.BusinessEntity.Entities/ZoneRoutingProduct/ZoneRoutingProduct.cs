using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ZoneRoutingProduct
    {
        public long ZoneId { get; set; }
        public int RoutingProductId { get; set; }
        public long SaleEntityZoneRoutingProductId { get; set; }
        public int CountryId { get; set; }
        public List<int> ServiceIds { get; set; }
        public bool IsInherited { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public DateTime CountryBED { get; set; }
        public DateTime? CountryEED { get; set; }
    }

    public class ZoneRoutingProductToEdit
    {
        public long ZoneId { get; set; }
        public int ChangedRoutingProductId { get; set; }
        public DateTime BED { get; set; }
        public int OwnerId { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
        public DateTime CountryBED { get; set; }
        public DateTime? CountryEED { get; set; }
    }

    public class ZoneRoutingProductToChange
    {
        public long ZoneRoutingProductId { get; set; }
        public DateTime EED { get; set; }
    }
}
