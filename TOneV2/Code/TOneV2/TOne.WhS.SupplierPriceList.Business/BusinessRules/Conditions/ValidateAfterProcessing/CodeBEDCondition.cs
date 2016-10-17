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

            if (importedCode.ChangeType == CodeChangeType.New || importedCode.ChangeType == CodeChangeType.Moved)
            {
                return (Vanrise.Common.ExtensionMethods.VRLessThanOrEqual(DateTime.Today.Date, importedCode.BED));
            }

            return true;
        }


        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has been opened in a period less than system period", (target as ImportedCode).Code);

        }
    }
}
