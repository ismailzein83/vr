﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class SupplierZoneRate
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal Rate { get; set; }
    }

    public class SupplierZoneRatesByZone : Dictionary<long, SupplierZoneRate>
    {

    }
}
