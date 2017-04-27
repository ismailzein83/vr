﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;


namespace Vanrise.NumberingPlan.Business
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

            var result = !(codeToAdd.ChangedExistingCodes != null && codeToAdd.ChangedExistingCodes.Count() > 0);

            if (result == false)
                context.Message = string.Format("Cannot add Code {0} because it's already exists", codeToAdd.Code);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
