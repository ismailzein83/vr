﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class MissingZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode importedCode = context.Target as ImportedCode;
            var result = !string.IsNullOrEmpty(importedCode.ZoneName);
            if (result == false)
            {
                context.Message = !string.IsNullOrEmpty(importedCode.Code)
                  ? string.Format("Code {0} has a missing zone name", importedCode.Code)
                  : "One of the records is missing zone name and code";
            }
            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
