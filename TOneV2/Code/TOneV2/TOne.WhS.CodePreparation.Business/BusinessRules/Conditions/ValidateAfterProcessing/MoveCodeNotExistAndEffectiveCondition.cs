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
    public class MoveCodeNotExistAndEffectiveCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CodeToMove != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CodeToMove codeToMove = context.Target as CodeToMove;
            var result = (codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToMove.Code
                         && item.ParentZone.ZoneEntity.Name.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase)));

            if (result == false)
               context.Message = string.Format("Can not move Code {0} because this code does not exist in zone {1}", codeToMove.Code, codeToMove.OldZoneName);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            CodeToMove codeToMove = target as CodeToMove;
            return string.Format("Code {0} does not exist in zone {1} and can not be moved", codeToMove.Code, codeToMove.OldZoneName);
        }
    }
}
