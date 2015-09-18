using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProductSettings
    {
        public SaleZoneGroup Zones { get; set; }

        public SuppliersGroup Suppliers { get; set; }
    }    
}
