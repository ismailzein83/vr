using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionSupplierInput
    {
        public int RoutingDatabaseId { get; set; }
        public int RoutingProductId { get; set; }
        public int SaleZoneId { get; set; }
        public int SupplierId { get; set; }
        public int? ToCurrencyId { get; set; }
        public decimal? SaleRate { get; set; }
    }
}
