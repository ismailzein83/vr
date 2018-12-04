using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions.RouteRules
{
    public class BySaleDealBuyRouteRule : DealBuyRouteRuleExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("43670B8E-C8D6-48DB-AE79-C8957DF1CF54"); } }

        public int DealId { get; set; }

        public List<long> SaleZoneIds { get; set; }

        public Decimal Percentage { get; set; }

        public override void Evaluate(IDealBuyRouteRuleEvaluateContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            string dealName = new SwapDealManager().GetDealName(DealId);
            List<string> zoneNames = this.SaleZoneIds != null ? new BusinessEntity.Business.SaleZoneManager().GetSaleZoneNames(this.SaleZoneIds) : null;
            string zonesDescription = zoneNames != null ? string.Format(" with Sale Zones: {0}", string.Join(", ", zoneNames)) : string.Empty;

            return string.Format("Deal '{0}'{1} (Percentage: {2}%)", dealName, zonesDescription, Percentage);
        }
    }
}
