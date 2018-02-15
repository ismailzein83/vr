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
    public class ImportedRateIsGreaterThanMaximumRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return (target is AllImportedZones);
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            AllImportedZones allImportedZones = context.Target as AllImportedZones;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            var invalidZones = new HashSet<string>();
            var priceListCurrencyId = importSPLContext.GetPriceListCurrencyId();
            var convertedMaximumRate = importSPLContext.GetMaximumRateConverted(priceListCurrencyId);
            foreach (var importedZone in allImportedZones.Zones)
            {
                if (importedZone.ImportedNormalRate != null)
                {
                    var currencyId = importSPLContext.GetImportedRateCurrencyId(importedZone.ImportedNormalRate);
                    if (importedZone.ImportedNormalRate.Rate > convertedMaximumRate)
                    {
                        invalidZones.Add(importedZone.ZoneName);
                    }
                }
            }
            
            if (invalidZones.Count > 0)
            {
                var generalSettingsManager = new Vanrise.Common.Business.GeneralSettingsManager();
                var longPrecisionValue = generalSettingsManager.GetLongPrecisionValue();
                context.Message = string.Format("Can not have rates greater than maximum rate '{0} ({1})'. Violated zone(s): '{2}'.", decimal.Round(convertedMaximumRate, longPrecisionValue), currencyManager.GetCurrencySymbol(priceListCurrencyId), string.Join(", ", invalidZones));
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
