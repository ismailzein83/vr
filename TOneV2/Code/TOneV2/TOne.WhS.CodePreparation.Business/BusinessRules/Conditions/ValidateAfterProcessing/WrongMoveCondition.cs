using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;


namespace TOne.WhS.CodePreparation.Business
{
    public class WrongMoveCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CodeToAdd != null && target as CodeToMove == null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CodeToAdd codeToAdd = context.Target as CodeToAdd;
            if (codeToAdd.ChangedExistingCodes != null)
            {
                if (codeToAdd.ChangedExistingCodes.Any(item => item.IsInTimeRange(codeToAdd.BED)))
                {
                    context.Message = string.Format("Can not add codes ({0}) because they already exist in zone '{1}'.", codeToAdd.Code, codeToAdd.ZoneName);
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
