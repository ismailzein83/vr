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
    public class CloseCodeNotExistAndEffectiveCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                if (codeToClose.ChangedExistingCodes.Count() == 0 || !codeToClose.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToClose.Code
                    && item.ParentZone.ZoneEntity.Name.ToLower().Equals(codeToClose.ZoneName.ToLower(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    context.Message = string.Format("Cannot close code {0} in zone {1} because this code either does not exist or it's not effective", codeToClose.Code, zoneToProcess.ZoneName);
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
