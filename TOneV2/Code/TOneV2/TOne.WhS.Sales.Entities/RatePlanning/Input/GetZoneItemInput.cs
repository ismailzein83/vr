using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneItemInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public long ZoneId { get; set; }
        public int RoutingDatabaseId { get; set; }
        public int PolicyConfigId { get; set; }
        public int NumberOfOptions { get; set; }
        public IEnumerable<CostCalculationMethod> CostCalculationMethods { get; set; }
    }
}
