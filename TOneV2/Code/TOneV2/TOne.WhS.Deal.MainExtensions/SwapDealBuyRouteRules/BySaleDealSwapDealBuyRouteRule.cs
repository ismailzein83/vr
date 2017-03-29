using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions.SwapDealBuyRouteRules
{
    public class BySaleDealSwapDealBuyRouteRule : SwapDealBuyRouteRuleExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public int SwapDealId { get; set; }

        public Decimal Percentage { get; set; }

        public override void Evaluate(ISwapDealBuyRouteRuleEvaluateContext context)
        {
            throw new NotImplementedException();
        }
    }
}
