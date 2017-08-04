using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
   public class SupplierTargetMatchFilter
    {
        public int SellingNumberPlanId { get; set; }
        public int RoutingProductId { get; set; }
        public int RoutingDataBaseId { get; set; }
        public IEnumerable<int> CountryIds { get; set; }
        public Guid PolicyId { get; set; }
        public int NumberOfOptions { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
