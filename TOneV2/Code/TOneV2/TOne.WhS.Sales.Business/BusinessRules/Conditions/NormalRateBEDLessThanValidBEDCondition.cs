using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NormalRateBEDLessThanValidBEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is DataByZone;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            bool result = true;
            var zone = context.Target as DataByZone;

            if (zone.NormalRateToChange != null)
            {
                if (zone.NormalRateToChange.BED == default(DateTime))
                    result = false;
                else
                {
                    var beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn };
                    if (zone.NormalRateToChange.BED < UtilitiesManager.GetMaxDate(beginEffectiveDates).Value)
                        result = false;
                }
            }

            if (result == false)
            {
                string zoneBED = UtilitiesManager.GetDateTimeAsString(zone.BED);
                string normalRateBED = UtilitiesManager.GetDateTimeAsString(zone.NormalRateToChange.BED);

                var messageBuilder = new StringBuilder(string.Format("BED '{0}' of the Normal Rate of Zone '{1}' must be greater than", normalRateBED, zone.ZoneName));

                if (zone.SoldOn.HasValue && zone.SoldOn > zone.BED)
                {
                    string soldOn = UtilitiesManager.GetDateTimeAsString(zone.SoldOn.Value);
                    messageBuilder.Append(string.Format(" the date '{0}' when the Country was sold", soldOn));
                }
                else
                {
                    messageBuilder.Append(string.Format(" the BED '{0}' of the Zone", zoneBED));
                }

                context.Message = messageBuilder.ToString();
            }

            return result;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
