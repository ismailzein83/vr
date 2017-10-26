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
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (!(codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToMove.Code
                         && item.ParentZone.ZoneEntity.Name.ToLower().Equals(codeToMove.OldZoneName.ToLower(), StringComparison.InvariantCultureIgnoreCase))))
                {
                    context.Message = string.Format("Cannot move Code {0} because it belongs to a pending closed zone : {1}", codeToMove.Code, codeToMove.OldZoneName);
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
