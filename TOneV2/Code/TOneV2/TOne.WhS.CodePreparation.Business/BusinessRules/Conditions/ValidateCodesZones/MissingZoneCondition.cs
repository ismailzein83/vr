﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
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
            if(result == false)
                context.Message = string.Format("Code {0} has a missing zone", importedCode.Code);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has a missing zone", (target as ImportedCode).Code);
        }
    }
}
