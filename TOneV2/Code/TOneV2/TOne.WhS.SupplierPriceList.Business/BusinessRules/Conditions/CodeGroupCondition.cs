﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeGroupCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("Target");

            ImportedCode code = context.Target as ImportedCode;
            return code.CodeGroup != null;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} does not belong to the code group of {1} zone", (target as ImportedCode).Code , (target as ImportedCode).ZoneName);
        }
    }
}
