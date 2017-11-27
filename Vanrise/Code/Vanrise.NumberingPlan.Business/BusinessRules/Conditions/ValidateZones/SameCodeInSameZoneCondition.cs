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
    public class SameCodeInSameZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            var invalidCodes = new HashSet<string>();

            foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
            {
                if (zoneToProcess.CodesToAdd.FindAllRecords(item => item.Code == codeToAdd.Code).Count() > 1)
                    invalidCodes.Add(codeToAdd.Code);
            }

            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                if (zoneToProcess.CodesToClose.FindAllRecords(item => item.Code == codeToClose.Code).Count() > 1)
                    invalidCodes.Add(codeToClose.Code);
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (codeToMove.ZoneName.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
                    invalidCodes.Add(codeToMove.Code);

            }
            if (invalidCodes.Count > 0)
            {
                context.Message += string.Format("Performing same action more than one time on code(s) ({0}) of zone '{1}'.", string.Join(", ", invalidCodes), zoneToProcess.ZoneName);
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
