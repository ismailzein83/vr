using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions.SwapDealBuyRouteRules
{
    public class FixedSwapDealBuyRouteRule : SwapDealBuyRouteRuleExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public int CustomerId { get; set; }

        public List<FixedSwapDealBuyRouteRuleZoneItem> SaleZones { get; set; }

        public Decimal DefaultPercentage { get; set; }

        public override void Evaluate(ISwapDealBuyRouteRuleEvaluateContext context)
        {
            context.EvaluationItems = this.SaleZones.Select(zoneItem =>
            new SwapDealBuyRouteRuleEvaluationItem
            {
                CustomerId = this.CustomerId,
                SaleZoneId = zoneItem.SaleZoneId,
                Percentage = zoneItem.Percentage.HasValue ? zoneItem.Percentage.Value : this.DefaultPercentage
            }).ToList();
        }
    }

    public class FixedSwapDealBuyRouteRuleZoneItem
    {
        public long SaleZoneId { get; set; }

        public Decimal? Percentage { get; set; }
    }
}
