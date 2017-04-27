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
                    context.Message = string.Format("Cannot close code {0} in zone {1} because this code is already pending closed", codeToClose.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                ExistingCode existingCodeToMove = codeToMove.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToMove.Code);

                if (existingCodeToMove != null && existingCodeToMove.CodeEntity.EED.HasValue)
                {
                    context.Message = string.Format("Cannot move code {0} in zone {1} because code is already pending closed", codeToMove.Code, zoneToProcess.ZoneName);
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
