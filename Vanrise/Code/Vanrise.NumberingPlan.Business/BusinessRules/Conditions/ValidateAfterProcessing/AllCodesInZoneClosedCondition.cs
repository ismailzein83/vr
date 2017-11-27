using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class AllCodesInZoneClosedCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CountryToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CountryToProcess countryToProcess = context.Target as CountryToProcess;
            var zonesToClose = new List<string>();
            foreach (var zone in countryToProcess.ZonesToProcess)
            {
                if (zone != null && zone.ChangeType == Entities.ZoneChangeType.Deleted)
                    zonesToClose.Add(zone.ZoneName);
            }

            if (zonesToClose.Count > 0)
            {
                context.Message = string.Format("Following zones are going to be closed: ({0}).", string.Join(", ", zonesToClose));
                return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
