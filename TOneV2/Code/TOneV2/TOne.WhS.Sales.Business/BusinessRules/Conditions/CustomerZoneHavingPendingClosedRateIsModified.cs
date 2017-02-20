using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class CustomerZoneHavingPendingClosedRateIsModified : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var zoneData = context.Target as DataByZone;

            if (zoneData.ZoneRateGroup != null && zoneData.ZoneRateGroup.NormalRate != null)
            {
                ZoneRate normalRate = zoneData.ZoneRateGroup.NormalRate;

                if (normalRate.Source == SalePriceListOwnerType.Customer && normalRate.EED.HasValue)
                {
                    context.Message = string.Format("Cannot modify SaleZone '{0}' because it has a pending closed SaleRate", zoneData.ZoneName);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            var zoneData = target as DataByZone;
            return string.Format("Cannot modify SaleZone '{0}' because it has a pending closed SaleRate", zoneData.ZoneName);
        }
    }
}
