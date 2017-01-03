using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountRecurringChargeEvaluators
{
    public class FixedChargeAccountRecurringChargeEvaluator : AccountRecurringChargeEvaluator
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Decimal Charge { get; set; }

        public override decimal Evaluate(IAccountRecurringChargeEvaluatorContext context)
        {
            return this.Charge;
        }
    }
}
