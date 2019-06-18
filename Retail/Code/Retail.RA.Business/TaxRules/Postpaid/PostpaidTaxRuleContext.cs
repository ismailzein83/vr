using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class PostpaidTaxRuleContext : IPostpaidTaxRuleContext
    {
        public PostpaidTaxRuleVoiceContext VoiceContext { get; set; }
        public PostpaidTaxRuleSMSContext SMSContext { get; set; }
        public PostpaidTaxRuleTransactionContext TransactionContext { get; set; }
        public IVRRule Rule { get; set; }
    }

    public interface IPostpaidTaxRuleContext : IRuleExecutionContext
    {
        PostpaidTaxRuleVoiceContext VoiceContext { get;}
        PostpaidTaxRuleSMSContext SMSContext { get; }
        PostpaidTaxRuleTransactionContext TransactionContext { get; }
    }

    public class PostpaidTaxRuleVoiceContext
    {
        public decimal? DurationInSeconds { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal TotalTaxValue { get; set; }
    }
    public class PostpaidTaxRuleSMSContext
    {
        public int? NumberOfSMS { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal TotalTaxValue { get; set; }

    }
    public class PostpaidTaxRuleTransactionContext
    {
        public decimal? TotalAmount { get; set; }
        public decimal TotalTaxValue { get; set; }
    }
}
