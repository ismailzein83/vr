using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business.BusinessRules
{
    public class ImportedOtherRateIsGreaterThanMaximumRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return (target is AllImportedZones);
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            AllImportedZones allImportedZones = context.Target as AllImportedZones;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            CurrencyManager currencyManager = new CurrencyManager();
            var priceListCurrencyId = importSPLContext.GetPriceListCurrencyId();
            var convertedMaximumRate = importSPLContext.GetMaximumRateConverted(priceListCurrencyId);

            var invalidZones = new HashSet<string>();

            foreach (var importedZone in allImportedZones.Zones)
            {
                if (importedZone.ImportedOtherRates.Values != null)
                {
                    foreach (ImportedRate otherImportedRate in importedZone.ImportedOtherRates.Values)
                    {
                        var currencyId = importSPLContext.GetImportedRateCurrencyId(importedZone.ImportedNormalRate);
                        if (otherImportedRate.Rate > convertedMaximumRate)
                        {
                            invalidZones.Add(importedZone.ZoneName);
                            break;
                        }
                    }
                }
            }

            if (invalidZones.Count > 1)
            {
                var generalSettingsManager = new Vanrise.Common.Business.GeneralSettingsManager();
                var longPrecisionValue = generalSettingsManager.GetLongPrecisionValue();
                context.Message = string.Format("Can not have other rates greater than maximum rate '{0} ({1})'. Violated zone(s): '{2}'.", decimal.Round(convertedMaximumRate, longPrecisionValue), currencyManager.GetCurrencySymbol(priceListCurrencyId), string.Join(", ", invalidZones));
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
