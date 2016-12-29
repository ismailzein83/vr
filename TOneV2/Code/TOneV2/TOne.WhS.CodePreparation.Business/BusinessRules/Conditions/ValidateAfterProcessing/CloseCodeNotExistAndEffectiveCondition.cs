using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
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

            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                if (codeToClose.ChangedExistingCodes.Count() == 0 || !codeToClose.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToClose.Code
                    && item.ParentZone.ZoneEntity.Name.Equals(codeToClose.ZoneName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Message = string.Format("Zone {0} has the code {1} to be closed that does not exist or not effective", zoneToProcess.ZoneName, codeToClose.Code);
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
