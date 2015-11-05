﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SaleEntityZoneRoutingProductSource { CustomerZone, CustomerDefault, ProductZone, ProductDefault }
    public class SaleEntityZoneRoutingProduct
    {

        public int RoutingProductId { get; set; }

        public SaleEntityZoneRoutingProductSource Source { get; set; }
    }
}
