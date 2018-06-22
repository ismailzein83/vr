using System;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.BusinessEntity.Business;
using System.Collections.Generic;

namespace TOne.WhS.Deal.BusinessProcessRules
{
    class RPValidateZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            //CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            //DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            //IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
            //var dataByZone = context.Target as DataByZone;
            //if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
            //{
            //    var customers = new CarrierAccountManager().GetCarrierAccountsAssignedToSellingProduct(ratePlanContext.OwnerId);
            //    List<string> zoneNames = new List<string>();
            //    foreach (var customer in customers)
            //    {
            //        if (dealDefinitionManager.IsSaleZoneIncludedInDeal(customer.CarrierAccountId, dataByZone.ZoneId, dataByZone.NormalRateToChange.BED))
            //        {
            //            zoneNames.Add(dataByZone.ZoneName);
            //            break;
            //        }
            //    }
            //}
            //else if ((dataByZone.NormalRateToChange != null || dataByZone.NormalRateToClose != null) && dealDefinitionManager.IsSaleZoneIncludedInDeal(ratePlanContext.OwnerId,dataByZone.ZoneId,dataByZone.NormalRateToChange.BED))
            //{
            //    context.Message = "";
            //    return false;
            //}
            return true;
        }
        private bool IsZoneLinkedToDeal(int customerId, long zoneId, DateTime effectiveAfter)
        {
            return false;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
