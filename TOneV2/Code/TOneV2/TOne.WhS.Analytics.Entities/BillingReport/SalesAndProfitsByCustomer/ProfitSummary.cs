﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class ProfitSummary
    {
        public double Profit { get; set; }
        public string FormattedProfit { get; set; }
        public string Customer { get; set; }
    }
}
