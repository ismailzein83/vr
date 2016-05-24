using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class CodeEEDCondition : BusinessRuleCondition
    {
       

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ExistingCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();

            ExistingCode existingCode = context.Target as ExistingCode;

            return (existingCode.ChangedCode == null ||
                    existingCode.ChangedCode.EED >= DateTime.Now.Add(importSplContext.CodeCloseDateOffset));
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has been closed in a period less than system period", (target as ExistingCode).CodeEntity.Code);

        }
    }
}
