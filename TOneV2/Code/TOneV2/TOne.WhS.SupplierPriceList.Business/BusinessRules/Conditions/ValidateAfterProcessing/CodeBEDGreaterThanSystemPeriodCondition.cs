using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    class CodeBEDGreaterThanSystemPeriodCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode importedCode = context.Target as ImportedCode;

            if (importedCode.ChangeType == CodeChangeType.New || importedCode.ChangeType == CodeChangeType.Moved)
            {
                IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();
                return (importedCode.BED > DateTime.Today.Add(importSplContext.CodeCloseDateOffset));
            }

            return true;
        }


        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has been opened in a period greater than system period", (target as ImportedCode).Code);

        }
    }
}
