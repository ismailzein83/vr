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
            return (target as CodeToMove != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CodeToMove codeToMove = context.Target as CodeToMove;
            return (codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToMove.Code
                        && item.ParentZone.ZoneEntity.Name.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase)));
        }

        public override string GetMessage(IRuleTarget target)
        {
            CodeToMove codeToMove = target as CodeToMove;
            return string.Format("Code {0} does not exist in zone {1} and can not be moved", codeToMove.Code, codeToMove.OldZoneName);
        }
    }
}
