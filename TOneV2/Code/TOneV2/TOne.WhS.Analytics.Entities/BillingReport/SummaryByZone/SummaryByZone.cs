﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class SummaryByZone
    {
        public string Zone { get; set; }

        public int? SupplierID { get; set; }

        public int Calls { get; set; }

        public double? Rate { get; set; }

        public decimal? DurationNet { get; set; }

        public byte RateType { get; set; }

        public decimal DurationInSeconds { get; set; }

        public double? Net { get; set; }

        public double? CommissionValue { get; set; }

        public double? ExtraChargeValue { get; set; }

    }
}
