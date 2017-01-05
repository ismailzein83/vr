using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountChargeEvaluators
{
    public class FixedChargeAccountChargeEvaluator : AccountChargeEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("8FEF1186-28BF-47FF-9C9B-3A2873F48F15"); }
        }

        public Decimal Charge { get; set; }

        public override decimal Evaluate(IAccountChargeEvaluatorContext context)
        {
            return this.Charge;
        }
    }
}
