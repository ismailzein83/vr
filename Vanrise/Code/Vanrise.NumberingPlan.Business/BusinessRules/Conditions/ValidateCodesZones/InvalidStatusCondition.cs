﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class InvalidStatusCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode importedData = context.Target as ImportedCode;
            var result = importedData.Status != ImportType.Invalid;
            if (result == false)
                context.Message = string.Format("Code {0} has an invalid status", importedData.Code);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
