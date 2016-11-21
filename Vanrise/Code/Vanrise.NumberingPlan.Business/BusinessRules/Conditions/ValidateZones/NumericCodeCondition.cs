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
            

            if(zoneToProcess.CodesToAdd != null)
            {
                foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToAdd.Code, 0))
                        return false;
                }
            }

            if (zoneToProcess.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToMove.Code, 0))
                        return false;
                }
            }

            if (zoneToProcess.CodesToClose != null)
            {
                foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToClose.Code, 0))
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
