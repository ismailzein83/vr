using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.RateType
{
    public class SpecificDayRateTypeSettings : PricingRuleRateTypeItemSettings
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("f87ee98b-673f-4095-8658-f6aa7e3966d3"); } }
        public DateTime Date { get; set; }

        protected override bool Evaluate(IPricingRuleRateTypeItemContext context)
        {
            return context.TargetTime.HasValue && context.TargetTime.Value.Date == this.Date.Date;
        }

        public override string GetDescription()
        {
            return (Date != null) ? String.Format("Date: {0}", Date.ToString()) : null;
        }
    }
}
