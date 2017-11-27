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
            var invalidCodes = new List<string>();
            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (!(codeToMove.ChangedExistingCodes != null && codeToMove.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToMove.Code
                         && item.ParentZone.ZoneEntity.Name.ToLower().Equals(codeToMove.OldZoneName.ToLower(), StringComparison.InvariantCultureIgnoreCase))))
                    invalidCodes.Add(codeToMove.Code);

            }
            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Can not move codes ({0}) to zone '{1}' because codes either do not exist or not effective.", string.Join(", ", invalidCodes), zoneToProcess.ZoneName);
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
