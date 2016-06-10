﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueRuleContext : IPricingRuleRateValueContext
    {
        public Decimal NormalRate { set; get; }

        public Dictionary<int, Decimal> RatesByRateType { set; get; }

        public BaseRule Rule { get; set; }
    }
}
