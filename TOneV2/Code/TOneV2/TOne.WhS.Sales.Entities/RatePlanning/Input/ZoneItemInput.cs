using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneItemsInput
    {
        public ZoneItemFilter Filter { get; set; }
        public int FromRow { get; set; }
        public int ToRow { get; set; }
    }

    public class ZoneItemFilter
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public char ZoneLetter { get; set; }
        public int RoutingDatabaseId { get; set; }
        public int RPRoutePolicyConfigId { get; set; }
        public int NumberOfOptions { get; set; }
        public List<CostCalculationMethod> CostCalculationMethods { get; set; }
        public IEnumerable<long> ZoneIds { get; set; } // This property was added so that the Rate Plan Changes page could use the same functionality to get the current zone rates and routing products
    }
}
