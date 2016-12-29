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
            bool result = true;
            if (importedCode.ChangeType == CodeChangeType.New || importedCode.ChangeType == CodeChangeType.Moved)
            {
                result = Vanrise.Common.ExtensionMethods.VRLessThanOrEqual(DateTime.Today.Date, importedCode.BED);
                if (result == false)
                    context.Message = string.Format("Code {0} has been opened in a period less than system period", importedCode.Code);
            }

            return result;
        }


        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has been opened in a period less than system period", (target as ImportedCode).Code);

        }
    }
}
