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
            return (target is AllImportedZones);
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            AllImportedZones allImportedZones = context.Target as AllImportedZones;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            var invalidZones = new HashSet<string>();

            foreach (var importedZone in allImportedZones.Zones)
            {
                if (importedZone.ImportedNormalRate != null)
                {
                    var currencyId = importSPLContext.GetImportedRateCurrencyId(importedZone.ImportedNormalRate);
                    var convertedMaximumRate = importSPLContext.GetMaximumRateConverted(currencyId);
                    if (importedZone.ImportedNormalRate.Rate > convertedMaximumRate)
                        invalidZones.Add(importedZone.ZoneName);
                }
            }

            if (invalidZones.Count > 0)
            {
                context.Message = string.Format("Can not have rates greater than maximum rate '{0}'. Violated zones are: '{1}'.", importSPLContext.MaximumRate, string.Join(", ", invalidZones));
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
