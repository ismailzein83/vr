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
                    context.Message = string.Format("Cannot close code {0} in zone {1} because this code is pending effective", codeToClose.Code, codeToClose.ZoneName);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.BED > DateTime.Today))
                {
                    context.Message = string.Format("Cannot move code {0} in zone {1} because this code is pending effective", codeToMove.Code, codeToMove.OldZoneName);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
