using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionSettings
    {
        public int SupplierId { get; set; }

        public int? Percentage { get; set; }

        public List<RegularRouteSupplierDeal> SupplierDeals { get; set; }

        public List<RouteBackupOptionSettings> Backups { get; set; }
    }

    public class RouteBackupOptionSettings
    {
        public int SupplierId { get; set; }
    }

    public class RegularRouteSupplierDeal : BaseRouteSupplierDeal
    {
    }
}