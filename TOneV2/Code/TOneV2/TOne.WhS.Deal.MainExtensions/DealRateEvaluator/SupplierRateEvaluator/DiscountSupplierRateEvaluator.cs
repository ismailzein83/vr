using System;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
    public class DiscountSupplierRateEvaluator : DealSaleRateEvaluator
    {
        public int Discount { get; set; }
        public override void EvaluateRate(IDealSaleRateEvaluatorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
