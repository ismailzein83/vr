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
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
            {
                if (!Vanrise.Common.Utilities.IsNumeric(codeToAdd.Code, 0))
                {
                    context.Message = string.Format("Can not add Code {0} to Zone {1} because this code is not a positive number", codeToAdd.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (!Vanrise.Common.Utilities.IsNumeric(codeToMove.Code, 0))
                {
                    context.Message = string.Format("Can not move Code {0} to Zone {1} because this code is not a positive number", codeToMove.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                if (!Vanrise.Common.Utilities.IsNumeric(codeToClose.Code, 0))
                {
                    context.Message = string.Format("Can not close Code {0} in Zone {1} because this code is not a positive number", codeToClose.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a Code that is not a positive number", (target as ZoneToProcess).ZoneName);
        }
    }
}
