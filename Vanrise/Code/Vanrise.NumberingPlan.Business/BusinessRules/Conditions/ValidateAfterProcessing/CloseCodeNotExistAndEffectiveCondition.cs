using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class CloseCodeNotExistAndEffectiveCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {


            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            if (zoneToProcess.CodesToClose != null)
            {
                foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
                {
                    if (codeToClose.ChangedExistingCodes.Count() == 0 || !codeToClose.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToClose.Code 
                        && item.ParentZone.ZoneEntity.Name.Equals(codeToClose.ZoneName, StringComparison.InvariantCultureIgnoreCase)))
                        return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a code to close that does not exist or not effective", (target as ZoneToProcess).ZoneName);
        }
    }
}
