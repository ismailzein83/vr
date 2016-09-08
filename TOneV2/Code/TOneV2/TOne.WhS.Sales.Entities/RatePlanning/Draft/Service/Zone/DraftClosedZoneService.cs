using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DraftClosedZoneService
    {
        public long ZoneId { get; set; }
        public int SaleEntityServiceId { get; set; }
        public DateTime EED { get; set; }
    }
}
