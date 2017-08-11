using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SupplierTargetMatch
    {
        public long SaleZoneId { get; set; }
        public string SaleZone { get; set; }
        public decimal Volume { get; set; }
        public decimal TargetVolume { get; set; }
        public IEnumerable<RPRouteOptionDetail> Options { get; set; }
        public List<SupplierTargetMatchAnalyticOption> TargetOptions { get; set; }
    }
}
