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
    public class ClosePendingClosedCodeCondition : BusinessRuleCondition
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
                ExistingCode existingCodeToClose = codeToClose.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToClose.Code);

                if (existingCodeToClose != null && existingCodeToClose.CodeEntity.EED.HasValue)
                {
                    context.Message = string.Format("Can not close code {0} at zone {1} because this code is already pending closed", codeToClose.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                ExistingCode existingCodeToMove = codeToMove.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToMove.Code);

                if (existingCodeToMove != null && existingCodeToMove.CodeEntity.EED.HasValue)
                {
                    context.Message = string.Format("Can not move code {0} at zone {1} because code is already pending closed", codeToMove.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a pending closed code that can not be moved or closed", (target as ZoneToProcess).ZoneName);
        }
    }
}
