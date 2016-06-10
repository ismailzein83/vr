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
            

            if(zoneToProcess.CodesToAdd != null)
            {
                foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToAdd.Code, false))
                        return false;
                }
            }

            if (zoneToProcess.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToMove.Code, false))
                        return false;
                }
            }

            if (zoneToProcess.CodesToClose != null)
            {
                foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
                {
                    if (!Vanrise.Common.Utilities.IsNumeric(codeToClose.Code, false))
                        return false;
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a Code which is not a numeric", (target as ZoneToProcess).ZoneName);
        }
    }
}
