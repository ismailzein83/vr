using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class DraftNewRateIsClosed : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var zoneData = context.Target as DataByZone;

            var invalidRateTypeNames = new List<string>();

            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.EED.HasValue)
            {
                invalidRateTypeNames.Add("Normal");
            }

            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
            {
                var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    if (otherRateToChange.EED.HasValue)
                    {
                        string rateTypeName = rateTypeManager.GetRateTypeName(otherRateToChange.RateTypeId.Value);
                        if (rateTypeName == null)
                            throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Name of RateType '{0}' was not found", otherRateToChange.RateTypeId.Value));
                        invalidRateTypeNames.Add(rateTypeName);
                    }
                }
            }

            if (invalidRateTypeNames.Count > 0)
            {
                context.Message = string.Format("New SaleRates '{0}' cannot be closed", string.Join(",", invalidRateTypeNames));
                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return "New SaleRates cannot be closed";
        }
    }
}
