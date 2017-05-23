using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NewRateIsGreaterThanMaximumRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return (target is DataByZone);
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            DataByZone zoneData = context.Target as DataByZone;
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.NormalRate > ratePlanContext.MaximumRate)
            {
                context.Message = string.Format("New normal rate '{0}' of zone '{1}' is greater than the maximum rate '{2}'", zoneData.NormalRateToChange.NormalRate, zoneData.ZoneName, ratePlanContext.MaximumRate);
                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
