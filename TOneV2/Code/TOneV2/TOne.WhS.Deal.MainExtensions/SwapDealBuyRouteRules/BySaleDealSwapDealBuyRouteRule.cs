using System;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions.SwapDealBuyRouteRules
{
    public class BySaleDealBuyRouteRule : DealBuyRouteRuleExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("43670B8E-C8D6-48DB-AE79-C8957DF1CF54"); } }

        public int DealId { get; set; }

        public Decimal Percentage { get; set; }

        public override void Evaluate(IDealBuyRouteRuleEvaluateContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            return string.Format("Deal '{0}':{1}%", new SwapDealManager().GetDealName(DealId), Percentage);
        }
    }
}
