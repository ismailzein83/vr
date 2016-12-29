using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
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
                    context.Message = string.Format("Zone {0} has a Code {1} that is not a positive number", zoneToProcess.ZoneName, codeToAdd.Code);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (!Vanrise.Common.Utilities.IsNumeric(codeToMove.Code, 0))
                {
                    context.Message = string.Format("Zone {0} has a Code {1} that is not a positive number", zoneToProcess.ZoneName, codeToMove.Code);
                    return false;
                }
            }

            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                if (!Vanrise.Common.Utilities.IsNumeric(codeToClose.Code, 0))
                {
                    context.Message = string.Format("Zone {0} has a Code {1} that is not a positive number", zoneToProcess.ZoneName, codeToClose.Code);
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
