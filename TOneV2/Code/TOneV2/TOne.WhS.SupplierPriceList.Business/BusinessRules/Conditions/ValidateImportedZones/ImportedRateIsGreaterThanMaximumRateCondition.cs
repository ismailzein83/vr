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
            var errorMessages = new List<string>();
            if (importedZone.ImportedNormalRate != null && importedZone.ImportedNormalRate.Rate > importSPLContext.MaximumRate)
            {
                errorMessages.Add(string.Format("Normal '{0}'", importedZone.ImportedNormalRate.Rate));
            }

            if (importedZone.ImportedOtherRates.Values != null)
            {
                var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

                foreach (ImportedRate otherImportedRate in importedZone.ImportedOtherRates.Values)
                {
                    if (otherImportedRate.Rate > importSPLContext.MaximumRate)
                    {
                        string rateTypeName = rateTypeManager.GetRateTypeName(otherImportedRate.RateTypeId.Value);
                        if (rateTypeName != null)
                            errorMessages.Add(string.Format("{0} '{1}'", rateTypeName, otherImportedRate.Rate));
                    }
                }
            }

            if (errorMessages.Count > 0)
            {
                context.Message = string.Format("The following rates of zone '{0}' are greater than the maximum rate '{1}': {2}", importedZone.ZoneName, importSPLContext.MaximumRate, string.Join(", ", errorMessages));
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
