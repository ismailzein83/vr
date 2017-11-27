using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class ZoneWithoutChangesCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllNewZones != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllNewZones allNewZones = context.Target as AllNewZones;
            var invalidZones = new List<string>();
            foreach (var newZone in allNewZones.Zones)
            {
                if (!newZone.hasChanges)
                    invalidZones.Add(newZone.Name);
            }
            if (invalidZones.Count > 0)
            {
                context.Message = string.Format("Can not create zone with out codes. Violated zone(s): {0}.", string.Join(", ", invalidZones));
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
