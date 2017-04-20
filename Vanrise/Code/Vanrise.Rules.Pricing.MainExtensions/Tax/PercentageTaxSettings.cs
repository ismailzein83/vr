using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.Tax
{
    public class PercentageTaxSettings : PricingRuleTaxActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("8C340085-E102-4504-A49B-329480CC7605"); }
        }

        public Decimal ExtraPercentage { get; set; }

        protected override void Execute(IPricingRuleTaxActionContext context)
        {
            throw new NotImplementedException();
        }
        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            if(this.FromAmount.HasValue)
            description.Append(String.Format("From Amount: {0}; ", FromAmount));
            if(this.ToAmount.HasValue)
             description.Append(String.Format("To Amount: {0}; ", ToAmount));
            description.Append(String.Format("Extra Percentage: {0}", ExtraPercentage));
            return description.ToString();
        }
    }
}
