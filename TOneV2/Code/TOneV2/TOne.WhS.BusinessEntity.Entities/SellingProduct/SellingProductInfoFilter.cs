﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SellingProductInfoFilter
    {
        public int? AssignableToCustomerId { get; set; }

        public int? SellingNumberPlanId { get; set; }
        public int? CurrencyId { get; set; }
    }
}
