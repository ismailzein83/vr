using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SaleZoneRoutingProductPreview
    {
        public string ZoneName { get; set; }

        public string CurrentSaleZoneRoutingProductName { get; set; }

        public bool? IsCurrentSaleZoneRoutingProductInherited { get; set; }

        public string NewSaleZoneRoutingProductName { get; set; }

        public DateTime EffectiveOn { get; set; }
    }

    public class SaleZoneRoutingProductPreviewDetail
    {
        public SaleZoneRoutingProductPreview Entity { get; set; }
    }
}
