using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class NumericCodeCondition : BusinessRuleCondition
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
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToAdd.Code, 0))
                        invalidCodes.Add(codeToAdd.Code);
                }
                foreach (var codeToClose in zone.CodesToClose)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToClose.Code, 0))
                        invalidCodes.Add(codeToClose.Code);
                }
                foreach (var codeToMove in zone.CodesToMove)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToMove.Code, 0))
                        invalidCodes.Add(codeToMove.Code);
                }
            }

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Codes have wrong format. Violated code(s): {0}.", string.Join(", ", invalidCodes));
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
