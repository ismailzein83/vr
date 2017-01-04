using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class ClosePendingEffectiveCodeCondition : BusinessRuleCondition
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
                if (codeToClose.ChangedExistingCodes != null && codeToClose.ChangedExistingCodes.Any(item => item.BED > DateTime.Today))
                {
                    string.Format("Can not close code {0} at zone {1} because code is pending effective", codeToClose.Code, zoneToProcess.RecentZoneName);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.BED > DateTime.Today))
                {
                    string.Format("Can not move code {0} at zone {1} because code is pending effective", codeToMove.Code, zoneToProcess.RecentZoneName);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a pending effective code that can not be closed or moved", (target as ZoneToProcess).RecentZoneName);
        }

    }
}
