using System;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public interface IVoiceTaxRuleContext : IRuleExecutionContext
    {
        Decimal DurationInSeconds { get; }

        Decimal? TotalAmount { get; }

        Decimal? TotalTaxValue { set; }
    }

    public class VoiceTaxRuleContext : IVoiceTaxRuleContext
    {
        public Decimal DurationInSeconds { get; set; }

        public Decimal? TotalAmount { get; set; }

        public Decimal? TotalTaxValue { set; get; }

        public IVRRule Rule { get; set; }
    }
}