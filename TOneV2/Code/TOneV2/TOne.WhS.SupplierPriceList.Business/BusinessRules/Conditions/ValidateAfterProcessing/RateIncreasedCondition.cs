using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using System.Linq;

namespace TOne.WhS.SupplierPriceList.Business
{
    class RateIncreasedCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as ImportedRate != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();

            ImportedRate importedRate = context.Target as ImportedRate;

           if(importedRate.ChangedExistingRates.Count() > 0 && importedRate.ChangedExistingRates.Any(item => item.RateEntity.NormalRate < importedRate.NormalRate))
               return Vanrise.Common.ExtensionMethods.VRLessThan(DateTime.Today.Add(importSplContext.CodeCloseDateOffset), importedRate.BED);

           return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("The rate of zone {0} has been increased in a period less than system period", (target as ImportedRate).ZoneName);
        }
    }
}
