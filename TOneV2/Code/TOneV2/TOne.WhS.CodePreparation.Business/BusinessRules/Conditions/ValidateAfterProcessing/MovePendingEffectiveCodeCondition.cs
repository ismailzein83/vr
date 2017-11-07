using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    class MovePendingEffectiveCodeCondition : BusinessRuleCondition
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
                invalidCodes.Add(codeToMove.Code);
            }

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Cannot move codes ({0}) to zone '{1}' because they are pending effective", string.Join(",", invalidCodes), zoneToProcess.ZoneName);
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
