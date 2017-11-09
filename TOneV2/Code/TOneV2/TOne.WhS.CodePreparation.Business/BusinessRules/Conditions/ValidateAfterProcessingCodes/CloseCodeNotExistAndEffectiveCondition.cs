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
            var invalidCodes = new List<string>();
            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                if (codeToClose.ChangedExistingCodes.Count() == 0 || !codeToClose.ChangedExistingCodes.Any(item => item.CodeEntity.Code == codeToClose.Code
                    && item.ParentZone.ZoneEntity.Name.ToLower().Equals(codeToClose.ZoneName.ToLower(), StringComparison.InvariantCultureIgnoreCase)))
                    invalidCodes.Add(codeToClose.Code);
            }

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Can not close codes ({0}) in zone '{1}' because codes either do not exist or not effective.", string.Join(",",invalidCodes), zoneToProcess.ZoneName);
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
