using System;
using System.Text;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using System.Collections.Generic;
using System.Linq;


namespace TOne.WhS.Deal.BusinessProcessRules
{
    class RPValidateRateChangeCondition : BusinessRuleCondition
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

            var zoneMessages = new List<String>();
            foreach (var dataByZone in allDataByZone.DataByZoneList)
            {
                if (dataByZone.NormalRateToChange != null || dataByZone.NormalRateToClose != null)
                {
                    var dealId = dealDefinitionManager.IsZoneIncludedInDeal(ratePlanContext.OwnerId, dataByZone.ZoneId, dataByZone.NormalRateToChange.BED, true);
                    if (dealId.HasValue)
                    {
                        var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                        var zoneName = new SaleZoneManager().GetSaleZoneName(dataByZone.ZoneId);
                        zoneMessages.Add(String.Format("zone {0} in deal {1}", zoneName, deal.Name));
                    }
                }
            }
            if (zoneMessages.Any())
            {
                string zoneMessagesString = string.Concat(",", zoneMessages);
                context.Message = String.Format("Modified rates cannot be done for zones included in deals. Following zones are : {0}", zoneMessagesString);
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
