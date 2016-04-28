using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class SameCodeInSameZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("Target");

            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
            {
                if (zoneToProcess.CodesToAdd.Where(x => x.Code == codeToAdd.Code).Count() > 1 
                    || zoneToProcess.CodesToClose.Where(x => x.Code == codeToAdd.Code).Count() > 0)
                    return false;
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has same code more than one time", (target as ZoneToProcess).ZoneName);
        }
    }
}
