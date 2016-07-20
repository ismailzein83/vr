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
            return (codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToMove.Code
                        && item.ParentZone.ZoneEntity.Name.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase)));
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Can not move code {0} that does not exist or not effective", (target as CodeToMove).Code);
        }
    }
}
