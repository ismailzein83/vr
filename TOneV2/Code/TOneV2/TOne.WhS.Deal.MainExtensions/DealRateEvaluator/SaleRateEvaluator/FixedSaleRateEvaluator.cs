using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
    public class FixedSaleRateEvaluator : DealSaleRateEvaluator
    {
        public Decimal Rate { get; set; }

        public override void EvaluateRate(IDealSaleRateEvaluatorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
