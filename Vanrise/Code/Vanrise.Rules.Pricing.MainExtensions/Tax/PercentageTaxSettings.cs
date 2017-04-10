using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.Tax
{
    public class PercentageTaxSettings : PricingRuleTaxSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Decimal TaxPercentage { get; set; }

        protected override void Execute(IPricingRuleTaxContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(GenericData.Entities.IGenericRuleSettingsDescriptionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
