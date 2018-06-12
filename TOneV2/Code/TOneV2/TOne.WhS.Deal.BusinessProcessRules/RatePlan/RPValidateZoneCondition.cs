using System;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Entities;

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
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
            var dataByZone = context.Target as DataByZone;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            if (IsZoneLinkedToDeal(ratePlanContext.OwnerId, dataByZone.ZoneId, dataByZone.BED))
            {
                context.Message = "";
                return false;
            }
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
