using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleTaxActionSettings
    {
        public abstract Guid ConfigId { get; }
        public Decimal? FromAmount { get; set; }
        public Decimal? ToAmount { get; set; }
        internal protected abstract void Execute(IPricingRuleTaxActionContext context);
        public abstract string GetDescription();
    }
}
