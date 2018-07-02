using System;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.BusinessEntity.Business;
using System.Collections.Generic;
using System.Text;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    class RPValidateZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllDataByZone;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var dealDefinitionManager = new DealDefinitionManager();
            var allDataByZone = context.Target as AllDataByZone;
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            StringBuilder sb = new StringBuilder();
            foreach (var dataByZone in allDataByZone.DataByZoneList)
            {
                if (dataByZone.NormalRateToChange != null || dataByZone.NormalRateToClose != null)
                {
                    var dealId = dealDefinitionManager.IsZoneIncludedInDeal(ratePlanContext.OwnerId, dataByZone.ZoneId, dataByZone.NormalRateToChange.BED, true);
                    if (dealId.HasValue)
                    {
                        var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                        var zoneName = new SaleZoneManager().GetSaleZoneName(dataByZone.ZoneId);
                        sb.AppendFormat("Zone '{0}' in deal '{1}'", zoneName, deal.Name);
                    }
                }
            }
            if (sb.Length > 1)
            {
                context.Message = String.Format("The Following zones are linked to deal : {0}", sb.ToString());
                return false;
            }
            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
