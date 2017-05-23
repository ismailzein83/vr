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

            var errorMessages = new List<string>();

            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.NormalRate > ratePlanContext.MaximumRate)
            {
                errorMessages.Add(string.Format("Normal '{0}'", zoneData.NormalRateToChange.NormalRate));
            }

            if (zoneData.OtherRatesToChange != null)
            {
                var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    if (otherRateToChange.NormalRate > ratePlanContext.MaximumRate)
                    {
                        string rateTypeName = rateTypeManager.GetRateTypeName(otherRateToChange.RateTypeId.Value);
                        if (rateTypeName != null)
                            errorMessages.Add(string.Format("{0} '{1}'", rateTypeName, otherRateToChange.NormalRate));
                    }
                }
            }

            if (errorMessages.Count > 0)
            {
                context.Message = string.Format("The following rates of zone '{0}' are greater than the maximum rate '{1}': {2}", zoneData.ZoneName, ratePlanContext.MaximumRate, string.Join(", ", errorMessages));
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
