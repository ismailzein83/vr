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
        public override string GetMessage(IRuleTarget target)
        {
            return String.Format("Cannot add a normal rate with BED < valid BED of zone {0}", (target as DataByZone).ZoneName);
        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            if (zone.NormalRateToChange != null)
            {
                var beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn };

                if (zone.NormalRateToChange.BED < UtilitiesManager.GetMaxDate(beginEffectiveDates).Value)
                    return false;
            }

            return true;
        }
    }
}
