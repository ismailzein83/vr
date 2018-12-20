using System;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public interface ISMSTaxRuleContext : IRuleExecutionContext
    {
        int NumberOfSMSs { get; }

        Decimal? TotalAmount { get; }

        Decimal? TotalTaxValue { set; }
    }

    public class SMSTaxRuleContext : ISMSTaxRuleContext
    {
        public int NumberOfSMSs { get; set; }

        public Decimal? TotalAmount { get; set; }

        public Decimal? TotalTaxValue { set; get; }

        public IVRRule Rule { get; set; }
    }
}