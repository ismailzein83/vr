using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions.RouteRules
{
    public class FixedDealBuyRouteRule : DealBuyRouteRuleExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("C49A0C3B-CE93-430B-BE81-8440EEBD87B0"); } }

        public int CustomerId { get; set; }

        public List<long> SaleZoneIds { get; set; }

        public Decimal Percentage { get; set; }

        public override void Evaluate(IDealBuyRouteRuleEvaluateContext context)
        {
            context.EvaluationItems = context.SaleZoneIds.Select(zoneId =>
            new DealBuyRouteRuleEvaluationItem
            {
                CustomerId = this.CustomerId,
                SaleZoneId = zoneId,
                Percentage = this.Percentage
            }).ToList();
        }

        public override string GetDescription()
        {
            string customerName = new CarrierAccountManager().GetCarrierAccountName(CustomerId);
            List<string> zoneNames = this.SaleZoneIds != null ? new SaleZoneManager().GetSaleZoneNames(this.SaleZoneIds) : null;
            string zonesDescription = zoneNames != null ? string.Format(" with Sale Zones: {0}", string.Join(", ", zoneNames)) : string.Empty;

            return string.Format("Customer '{0}'{1} (Percentage: {2}%)", customerName, zonesDescription, Percentage);
        }
    }
}