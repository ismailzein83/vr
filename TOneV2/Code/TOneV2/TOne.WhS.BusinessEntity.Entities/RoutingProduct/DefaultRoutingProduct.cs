﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class DefaultRoutingProduct
    {
        public long SaleEntityRoutingProductId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int RoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
