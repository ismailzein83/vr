﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleZoneInfoFilter
    {
        public int SellingNumberPlanId { get; set; }

        public ISaleZoneGroupContext SaleZoneGroupContext { get; set; }
    }
}
