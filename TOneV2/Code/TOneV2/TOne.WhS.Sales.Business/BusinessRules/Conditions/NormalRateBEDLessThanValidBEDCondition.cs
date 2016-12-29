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
            return (target as DataByZone != null);
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

                if (zone.SoldOn.HasValue)
                {
                    string soldOn = UtilitiesManager.GetDateTimeAsString(zone.SoldOn.Value);
                    messageBuilder.Append(string.Format(" the maximum between the BED '{0}' of the Zone and the date '{1}' when the Country was sold", zoneBED, soldOn));
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
            DataByZone zone = (target as DataByZone);

            string zoneBED = UtilitiesManager.GetDateTimeAsString(zone.BED);
            string normalRateBED = UtilitiesManager.GetDateTimeAsString(zone.NormalRateToChange.BED);

            var messageBuilder = new StringBuilder(string.Format("BED '{0}' of the Normal Rate of Zone '{1}' must be greater than", normalRateBED, zone.ZoneName));

            if (zone.SoldOn.HasValue)
            {
                string soldOn = UtilitiesManager.GetDateTimeAsString(zone.SoldOn.Value);
                messageBuilder.Append(string.Format(" the maximum between the BED '{0}' of the Zone and the date '{1}' when the Country was sold", zoneBED, soldOn));
            }
            else
            {
                messageBuilder.Append(string.Format(" the BED '{0}' of the Zone", zoneBED));
            }

            return messageBuilder.ToString();
        }
    }
}
