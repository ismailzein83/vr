using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleTaxContext : IRuleExecutionContext
    {
        DateTime? TargetTime { get; }

        Decimal Amount { get; set; }

        Decimal TaxAmount { set; }
        decimal Percentage { set; }

        int CurrencyId { get; }
    }
}
