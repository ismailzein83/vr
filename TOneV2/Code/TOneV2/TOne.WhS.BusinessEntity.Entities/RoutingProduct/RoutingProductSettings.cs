using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProductSettings
    {
        public SaleZoneGroupSettings Zones { get; set; }

        public SuppliersGroupSettings Suppliers { get; set; }
    }    
}
