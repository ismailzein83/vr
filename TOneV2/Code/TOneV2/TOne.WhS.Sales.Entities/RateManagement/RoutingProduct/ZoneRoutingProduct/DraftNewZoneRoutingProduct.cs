﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DraftNewZoneRoutingProduct
    {
        public long ZoneId { get; set; }

        public int ZoneRoutingProductId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
