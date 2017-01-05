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
            get { throw new NotImplementedException(); }
        }

        public Decimal Charge { get; set; }

        public override decimal Evaluate(IAccountChargeEvaluatorContext context)
        {
            return this.Charge;
        }
    }
}
