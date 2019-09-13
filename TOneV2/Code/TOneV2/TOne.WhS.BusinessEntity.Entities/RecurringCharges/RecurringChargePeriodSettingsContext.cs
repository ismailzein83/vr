﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RecurringChargePeriodSettingsContext : IRecurringChargePeriodSettingsContext
    {
        public DateTime RecurringChargeBED { set; get; }
        public DateTime FromDate { set; get; }
        public DateTime ToDate { set; get; }
        public List<RecurringChargePeriodOutput> Periods { set; get; }
    }

    public class RecurringChargePeriodOutput
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime RecurringChargeDate { get; set; }
    }
}
