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

            return !(importedRate.ChangedExistingRates.Count() > 0 && importedRate.ChangedExistingRates.Any(item => item.RateEntity.NormalRate < importedRate.Rate));
        }
        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("The rate of zone {0} has been increased", (target as ImportedRate).ZoneName);
        }
    }
}
