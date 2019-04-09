using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class TransactionTaxRuleContext : ITransactionTaxRuleContext
    {
        public Decimal? TotalAmount { get; set; }

        public Decimal? TotalTaxValue { set; get; }

        public IVRRule Rule { get; set; }
    }

    public interface ITransactionTaxRuleContext : IRuleExecutionContext
    {
        Decimal? TotalAmount { get; }
        Decimal? TotalTaxValue { set; }
    }
}
