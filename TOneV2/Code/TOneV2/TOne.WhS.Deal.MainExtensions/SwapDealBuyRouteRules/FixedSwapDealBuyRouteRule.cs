using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions.SwapDealBuyRouteRules
{
    public class FixedSwapDealBuyRouteRule : SwapDealBuyRouteRuleExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("C49A0C3B-CE93-430B-BE81-8440EEBD87B0"); } }

        public int CustomerId { get; set; }

        public Decimal Percentage { get; set; }

        public override void Evaluate(ISwapDealBuyRouteRuleEvaluateContext context)
        {
            context.EvaluationItems = context.SaleZoneIds.Select(zoneId =>
            new SwapDealBuyRouteRuleEvaluationItem
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