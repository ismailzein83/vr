using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Common;

namespace Vanrise.NumberingPlan.Business
{
    class MovePendingClosedCodeCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            var invalidCodes = new HashSet<string>();

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                ExistingCode existingCodeToMove = codeToMove.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToMove.Code);
                if (existingCodeToMove != null && existingCodeToMove.CodeEntity.EED.HasValue)
                    invalidCodes.Add(codeToMove.Code);
            }

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Can not move codes ({0}) to zone '{1}' because codes are pending closed.", string.Join(",", invalidCodes), zoneToProcess.ZoneName);
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
