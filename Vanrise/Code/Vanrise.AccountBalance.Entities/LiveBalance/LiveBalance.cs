﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalance
    {
        public long AccountId { get; set; }
        public decimal InitialBalance { get; set; }
        public Decimal UsageBalance { get; set; }
        public Decimal CurrentAlertThreshold { get; set; }
        public Decimal NextAlertThreshold { get; set; }
        public long AlertRuleID { get; set; }
        public Decimal CurrentBalance { get; set; }
        public int CurrencyId { get; set; }
    }
}
