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
    public class SameCodeInSameZoneCondition : BusinessRuleCondition
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
                if (zoneToProcess.CodesToAdd.FindAllRecords(item => item.Code == codeToAdd.Code).Count() > 1)
                {
                    context.Message = string.Format("Cannot add Code {0} because Zone {1} contains this Code.", codeToAdd.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                if (zoneToProcess.CodesToClose.FindAllRecords(item => item.Code == codeToClose.Code).Count() > 1)
                {
                    context.Message = string.Format("Cannot close Code {0} because Zone {1} contains this Code.", codeToClose.Code, zoneToProcess.ZoneName);
                    return false;
                }
            }

            foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
            {
                if (codeToMove.ZoneName.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Message = string.Format("Cannot move Code {0} because Zone {1} contains this Code.", codeToMove.Code, zoneToProcess.ZoneName);
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
