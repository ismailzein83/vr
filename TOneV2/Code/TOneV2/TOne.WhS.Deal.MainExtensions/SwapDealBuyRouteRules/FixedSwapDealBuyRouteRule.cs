using System;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions.SwapDealBuyRouteRules
{
    public class FixedDealBuyRouteRule : DealBuyRouteRuleExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("C49A0C3B-CE93-430B-BE81-8440EEBD87B0"); } }

        public int CustomerId { get; set; }

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
            return string.Format("Customer '{0}':{1}%", new CarrierAccountManager().GetCarrierAccountName(CustomerId), Percentage);
        }
    }
}