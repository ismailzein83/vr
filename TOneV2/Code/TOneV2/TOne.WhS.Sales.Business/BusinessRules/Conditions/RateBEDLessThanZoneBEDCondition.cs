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
    public class RateBEDLessThanZoneBEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            DataByZone zone = context.Target as DataByZone;

            if (zone.RateToChange != null && zone.RateToChange.BED < zone.BED)
                return false;

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Trying to update a rate with BED less than BED of zone {0}", (target as DataByZone).ZoneName);
        }
    }
}
