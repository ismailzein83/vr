﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
   public class RateAnalysis
    {
        public Int64 RateID { get; set; }

        public Decimal Rate { get; set; }
        public Decimal OffPeakRate { get; set; }
        public Decimal WeekendRate { get; set; }
        public Change Change { get; set; }
        public Int16 ServicesFlag { get; set; }
        public DateTime BeginEffectiveDate { get; set; }
        public string Currency { get; set; }
        public DateTime EndEffectiveDate { get; set; }
        public IsEffective Effective { get; set; }
        public string Notes { get; set; }
    }
}
