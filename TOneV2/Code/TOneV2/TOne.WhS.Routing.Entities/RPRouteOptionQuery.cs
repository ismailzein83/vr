using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPRouteOptionQuery
    {
        public int RoutingDatabaseId { get; set; }

        public Guid PolicyOptionConfigId { get; set; }

        public int RoutingProductId { get; set; }

        public int SaleZoneId { get; set; }

        public int? CustomerId { get; set; }

        public bool ShowInSystemCurrency { get; set; }

        public bool IncludeBlockedSuppliers { get; set; }
    }
}
