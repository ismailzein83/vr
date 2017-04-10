using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleTaxContext
    {
        Decimal OriginalAmount { get; }

        int CurrencyId { get; }

        Decimal AmountWithTaxes { set; }
    }
}
