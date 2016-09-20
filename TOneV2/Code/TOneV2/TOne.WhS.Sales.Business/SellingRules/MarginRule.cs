using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.SellingRules
{
    public class MarginRule : SellingRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("04fdcf7e-ba4f-4274-8da8-3c40db867170"); } }

        public decimal FromRate { get; set; }
        public decimal ToRate { get; set; }
        public decimal MinMargin { get; set; }
        public decimal MaxMargin { get; set; }
        public bool IsPercentage { get; set; }
        public override void Execute(ISellingRuleExecutionContext context)
        {
            if (IsPercentage)
            {
                decimal fromCustomerRateValue = context.CustomerRate.HasValue ? context.CustomerRate.Value + (context.CustomerRate.Value * MinMargin / 100) : 0;
                decimal toCustomerRateValue = context.CustomerRate.HasValue ? context.CustomerRate.Value + (context.CustomerRate.Value * MaxMargin / 100) : 0;
                decimal fromProductRateValue = context.ProductRate.HasValue ? context.ProductRate.Value + (context.ProductRate.Value * MinMargin / 100) : 0;
                decimal toProductRateValue = context.ProductRate.HasValue ? context.ProductRate.Value + (context.ProductRate.Value * MaxMargin / 100) : 0;

                if ((context.CustomerRate.HasValue && fromCustomerRateValue >= FromRate && toCustomerRateValue <= ToRate)
                    ||(context.ProductRate.HasValue && fromProductRateValue >= FromRate && toProductRateValue <= ToRate))
                {
                    context.Status = MarginStatus.Valid;
                    return;
                }
                else
                {
                    context.Status = MarginStatus.Invalid;
                    return;
                }
            }
            else
            {

                decimal fromCustomerRateValue = context.CustomerRate.HasValue ? context.CustomerRate.Value + MinMargin : 0;
                decimal toCustomerRateValue = context.CustomerRate.HasValue ? context.CustomerRate.Value + MaxMargin : 0;
                decimal fromProductRateValue = context.ProductRate.HasValue ? context.ProductRate.Value + MinMargin : 0;
                decimal toProductRateValue = context.ProductRate.HasValue ? context.ProductRate.Value + MaxMargin : 0;

                if ((context.CustomerRate.HasValue && fromCustomerRateValue >= FromRate && toCustomerRateValue <= ToRate)
                || (context.ProductRate.HasValue && fromProductRateValue >= FromRate && toProductRateValue <= ToRate))
                {
                    context.Status = MarginStatus.Valid;
                    return;
                }
                context.Status = MarginStatus.Invalid;
            }
        }
    }
}
