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
        public decimal? TotalVoiceAmount { get; set; }
        public decimal? TotalSMSAmount { get; set; }
        public decimal? TotalTransactionAmount { get; set; }
        public decimal? TotalTaxValue { set; get; }
        public IVRRule Rule { get; set; }
    }

    public interface IPostpaidTaxRuleContext : IRuleExecutionContext
    {
        decimal? TotalVoiceAmount { get; }
        decimal? TotalSMSAmount { get; }
        decimal? TotalTransactionAmount { get; }
        decimal? TotalTaxValue { get; set; }
    }
}
