using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RatePlanning
{
    public class ZoneChanges
    {
        public long ZoneId { get; set; }

        public NewRate NewRate { get; set; }

        public RateChange RateChange { get; set; }

        public NewZoneRoutingProduct NewRoutingProduct { get; set; }

        public ZoneRoutingProductChange RoutingProductChange { get; set; }

        public NewService NewService { get; set; }

        public ServiceChange ServiceChange { get; set; }
    }
}
