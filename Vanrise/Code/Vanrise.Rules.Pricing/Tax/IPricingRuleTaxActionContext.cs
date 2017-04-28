using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleTaxActionContext
    {
        DateTime? TargetTime { get; }

        Decimal Amount { get; }

        int RuleCurrencyId { get; }

        int AmountCurrencyId { get; }
        decimal? TaxAmount { set; }
    }
}
