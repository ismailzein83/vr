using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions.SwapDeal.RouteRuleCriteria
{
    public class SwapDealRouteRuleCriteria : BaseRouteRuleCriteria
    {
        public override Guid ConfigId { get { return new Guid("0CE291EB-790F-4B24-9DC1-512D457546C5"); } }
        public int SwapDealId { get; set; }
        public List<long> ZoneIds { get; set; }

        public override BusinessEntity.Entities.SaleZoneGroupSettings GetSaleZoneGroupSettings()
        {
            SwapDealSettings swapDealSettings = new SwapDealManager().GetDealSettings<SwapDealSettings>(SwapDealId);

            CarrierAccount carrierAccount = new CarrierAccountManager().GetCarrierAccount(swapDealSettings.CarrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", swapDealSettings.CarrierAccountId);

            int? sellingNumberPlan = carrierAccount.SellingNumberPlanId;
            if (!sellingNumberPlan.HasValue)
                throw new NullReferenceException(string.Format("sellingNumberPlan. CarrierAccountId: '{0}'", swapDealSettings.CarrierAccountId));

            return new TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.SelectiveSaleZoneGroup() { ZoneIds = ZoneIds, SellingNumberPlanId = sellingNumberPlan.Value };
        }

        public override bool IsVisibleInManagementView()
        {
            return false;
        }
    }
}