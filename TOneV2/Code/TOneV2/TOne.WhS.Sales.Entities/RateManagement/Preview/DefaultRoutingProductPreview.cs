using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultRoutingProductPreview
    {
        public string CurrentDefaultRoutingProductName { get; set; }

        public bool? IsCurrentDefaultRoutingProductInherited { get; set; }

        public string NewDefaultRoutingProductName { get; set; }

        public DateTime EffectiveOn { get; set; }
    }
}
