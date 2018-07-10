using System;
using System.Linq;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Deal.Business;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;


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
            var allDataByZone = context.Target as AllDataByZone;
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var zoneMessages = new List<String>();
            foreach (var dataByZone in allDataByZone.DataByZoneList)
            {
                if (dataByZone.NormalRateToChange == null && dataByZone.NormalRateToClose == null)
                    return true;

                DateTime effectiveDate = dataByZone.NormalRateToChange != null
                    ? dataByZone.NormalRateToChange.BED
                    : dataByZone.NormalRateToClose.CloseEffectiveDate;

                var zoneName = new SaleZoneManager().GetSaleZoneName(dataByZone.ZoneId);
                string dealMessage = Helper.GetDealZoneMessage(ratePlanContext.OwnerId, dataByZone.ZoneId, zoneName, effectiveDate, true);
                if (dealMessage != null)
                    zoneMessages.Add(dealMessage);

            }
            if (zoneMessages.Any())
            {
                string zoneMessagesString = string.Join(",", zoneMessages);
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
