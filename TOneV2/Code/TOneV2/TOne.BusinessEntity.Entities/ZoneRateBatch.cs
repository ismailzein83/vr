﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class ZoneRateBatch
    {
        public bool IsSupplierZoneRateBatch { get; set; }

        public List<ZoneRate> ZoneRates { get; set; }
    }
}
