using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneRoutingProductChangesDetail
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string CurrentRoutingProductName { get; set; }
        public bool? IsCurrentRoutingProductInherited { get; set; }
        public string NewRoutingProductName { get; set; }
        public bool IsNewRoutingProductInherited { get; set; }
        public DateTime EffectiveOn { get; set; }
    }
}
