using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class PrepaidTaxRuleContext : IPrepaidTaxRuleContext
    {
        public TransactionTaxRuleContext TopUpContext { get; set; }
        public PrepaidResidualTaxRuleContext ResidualContext { get; set; }
        public IVRRule Rule { get; set; }
    }

    public interface IPrepaidTaxRuleContext : IRuleExecutionContext
    {
        TransactionTaxRuleContext TopUpContext { get; }
        PrepaidResidualTaxRuleContext ResidualContext { get; }
    }

    public class PrepaidResidualTaxRuleContext
    {
    }
}
