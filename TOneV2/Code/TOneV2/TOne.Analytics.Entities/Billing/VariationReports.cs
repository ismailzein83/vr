﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class VariationReports
    {
        public string Name { get; set; }

        public decimal PeriodTypeValueAverage { get; set; }

        public decimal PeriodTypeValuePercentage { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal TotalDuration { get; set; }

        public decimal PreviousPeriodTypeValuePercentage { get; set; }

        public string ID { get; set; }




    }
}
