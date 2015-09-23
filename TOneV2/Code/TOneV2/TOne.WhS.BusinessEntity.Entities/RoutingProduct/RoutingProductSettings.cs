using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RoutingProductSettings
    {
        public int? SaleZoneGroupConfigId { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public int? SupplierGroupConfigId { get; set; }

        public SupplierGroupSettings SupplierGroupSettings { get; set; }
    }    
}
