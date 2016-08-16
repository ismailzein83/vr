using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    class CodeEEDCondition : BusinessRuleCondition
    {
       

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as NotImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            NotImportedCode notImportedCode = context.Target as NotImportedCode;
            if (!notImportedCode.HasChanged)
                return true;

            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();
            return !notImportedCode.EED.VRLessThanOrEqual(DateTime.Today.Add(importSplContext.CodeCloseDateOffset));
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has been closed in a period less than system period", (target as NotImportedCode).Code);
        }
    }
}
