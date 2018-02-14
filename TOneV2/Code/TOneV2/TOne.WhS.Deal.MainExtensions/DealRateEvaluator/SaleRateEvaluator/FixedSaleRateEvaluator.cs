using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
    public class FixedSaleRateEvaluator : DealRateEvaluator
    {
        public Decimal Rate { get; set; }

        public override void EvaluateRate(IDealRateEvaluatorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
