using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProductSettings
    {
        public int SaleZoneGroupConfigId { get; set; }

        public SaleZoneGroupSettings Zones { get; set; }

        public int SuppliersGroupConfigId { get; set; }

        public SuppliersGroupSettings Suppliers { get; set; }
    }    
}
