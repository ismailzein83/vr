﻿using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class CodeBEDCondition : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has been opened in a period less than system period", (target as ICode).Code);

        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is ICode;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("Target");
            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();
            ICode code = context.Target as ICode;
            return (code.BED < DateTime.Now.Add(importSplContext.CodeCloseDateOffset));
        }
    }
}
