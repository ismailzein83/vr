using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class CodeBEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode importedCode = context.Target as ImportedCode;

            if (importedCode.ChangeType == CodeChangeType.NotChanged || importedCode.ChangeType == CodeChangeType.Deleted)
                return true;

            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();

            return (importedCode.BED >= DateTime.Now.Add(importSplContext.CodeCloseDateOffset));
        }


        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has been opened in a period less than system period", (target as ImportedCode).Code);

        }
    }
}
