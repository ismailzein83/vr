using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class CodeGroupCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllZonesToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            AllZonesToProcess allZonesToProcess = context.Target as AllZonesToProcess;
            var invalidCodes = new HashSet<string>();

            foreach (var zone in allZonesToProcess.Zones)
            {
                foreach (var codeToAdd in zone.CodesToAdd)
                {
                    if (codeToAdd != null && codeToAdd.CodeGroup == null)
                        invalidCodes.Add(codeToAdd.Code);
                }
                foreach (var codeToClose in zone.CodesToClose)
                {
                    if (codeToClose != null && codeToClose.CodeGroup == null)
                        invalidCodes.Add(codeToClose.Code);
                }
            }

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("No code group defined for the following code(s): {0}.", string.Join(", ", invalidCodes));
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
