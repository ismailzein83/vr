using System;
using System.Collections.Generic;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public interface ISMSRateValueRuleContext : IRuleExecutionContext
    {
        int CurrencyId { get; set; }

        Decimal NormalRate { get; set; }

        Dictionary<int, Decimal> RatesByRateType { set; get; }
    }

    public class SMSRateValueRuleContext : ISMSRateValueRuleContext
    {
        public int CurrencyId { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> RatesByRateType { set; get; }

        public IVRRule Rule { get; set; }
    }
}
