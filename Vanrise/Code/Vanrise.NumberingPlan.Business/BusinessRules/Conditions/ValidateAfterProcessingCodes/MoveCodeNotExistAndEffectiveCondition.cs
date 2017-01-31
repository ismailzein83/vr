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
    public class MoveCodeNotExistAndEffectiveCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (!(codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToMove.Code
                         && item.ParentZone.ZoneEntity.Name.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase))))
                {
                    context.Message = string.Format("Can not move Code {0} because this code does not exist in zone {1}", codeToMove.Code, codeToMove.OldZoneName);
                    return false;
                }

            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            CodeToMove codeToMove = target as CodeToMove;
            return string.Format("Code {0} does not exist in zone {1} and can not be moved", codeToMove.Code, codeToMove.OldZoneName);
        }
    }
}
