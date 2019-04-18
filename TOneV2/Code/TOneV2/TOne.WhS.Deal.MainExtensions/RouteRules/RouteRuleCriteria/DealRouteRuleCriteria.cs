﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions.RouteRule.RouteRuleCriteria
{
    public class DealRouteRuleCriteria : BaseRouteRuleCriteria
    {
        public override Guid ConfigId { get { return new Guid("0CE291EB-790F-4B24-9DC1-512D457546C5"); } }
        public int DealId { get; set; }
        public List<long> ZoneIds { get; set; }

        public override int? GetDealId() { return this.DealId; }

        public override BusinessEntity.Entities.SaleZoneGroupSettings GetSaleZoneGroupSettings()
        {
            if (this.ZoneIds == null || this.ZoneIds.Count == 0)
                return null;

            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            DealDefinition dealDefinition = dealDefinitionManager.GetDeal(this.DealId);
            dealDefinition.ThrowIfNull("dealDefinition", this.DealId);
            dealDefinition.Settings.ThrowIfNull("dealDefinition.Settings", this.DealId);

            int carrierAccountId = dealDefinition.Settings.GetCarrierAccountId();
            CarrierAccount carrierAccount = new CarrierAccountManager().GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);

            int? sellingNumberPlan = carrierAccount.SellingNumberPlanId;
            if (!sellingNumberPlan.HasValue)
                throw new NullReferenceException(string.Format("sellingNumberPlan. CarrierAccountId: '{0}'", carrierAccountId));

            return new TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.SelectiveSaleZoneGroup() { ZoneIds = this.ZoneIds, SellingNumberPlanId = sellingNumberPlan.Value };
        }

        public override bool IsVisibleInManagementView()
        {
            return false;
        }
    }
}