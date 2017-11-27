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
    public class ClosePendingClosedCodeCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            var invalidCodes = new HashSet<string>();

            foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
            {
                ExistingCode existingCodeToClose = codeToClose.ChangedExistingCodes.FindRecord(item => item.CodeEntity.Code == codeToClose.Code);
                if (existingCodeToClose != null && existingCodeToClose.CodeEntity.EED.HasValue)
                    invalidCodes.Add(codeToClose.Code);
            }
            
            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Can not close codes ({0}) in zone '{1}' because codes are pending closed.", string.Join(",", invalidCodes), zoneToProcess.ZoneName);
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
