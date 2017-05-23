using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business.BusinessRules
{
    public class ImportedRateIsGreaterThanMaximumRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return (target is ImportedZone);
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            ImportedZone importedZone = context.Target as ImportedZone;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            if (importedZone.ImportedNormalRate != null && importedZone.ImportedNormalRate.Rate > importSPLContext.MaximumRate)
            {
                context.Message = string.Format("Imported normal rate '{0}' of zone '{1}' is greater than the maximum rate '{2}'", importedZone.ImportedNormalRate.Rate, importedZone.ZoneName, importSPLContext.MaximumRate);
                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
