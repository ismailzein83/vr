﻿using System;
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

            if (zoneToProcess.CodesToClose != null)
            {
                foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
                {
                    ExistingCode existingCodeToClose = codeToClose.ChangedExistingCodes.FindRecord(item=>item.CodeEntity.Code == codeToClose.Code);

                    if (existingCodeToClose != null && existingCodeToClose.CodeEntity.EED.HasValue)
                        return false;
                }

            }


            if (zoneToProcess.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
                {
                    ExistingCode existingCodeToMove = codeToMove.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToMove.Code);

                    if (existingCodeToMove != null && existingCodeToMove.CodeEntity.EED.HasValue)
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
