using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class AcceptableRateDecreaseCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllImportedCountries;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var configManager = new ConfigManager();
            int acceptableDecreaseRatePercentage = configManager.GetPurchaseAcceptableDecreasedRate();

            int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
            bool isValid = true;
            var allImportedCountries = context.Target as AllImportedCountries;
            
            var contextMessages = new List<string>();
          
            foreach (var importedCountry in allImportedCountries.ImportedCountries)
            {
                var numberOfAcceptableDecreasedRate = 0;
                var zonesWithDecreasedRatesAboveAcceptable = new List<string>();

                if (importedCountry.ImportedRates == null || importedCountry.ImportedRates.Count == 0)
                    continue;

               
             

                foreach (ImportedRate importedRate in importedCountry.ImportedRates)
                {
                    if (importedRate.ChangeType == RateChangeType.Decrease)
                    {
                        importedRate.ProcessInfo.RecentExistingRate.ThrowIfNull("RecentExistingRate");

                        var recentRate = importedRate.ProcessInfo.RecentExistingRate.ConvertedRate;
                        var newRate = importedRate.Rate;
                        var recentRateFormatted = Decimal.Round(recentRate, longPrecision);
                        var newRateFormatted = Decimal.Round(newRate, longPrecision);

                        var decreasePercentage = ((1 - newRate / recentRate) * 100);

                        if (acceptableDecreaseRatePercentage < decreasePercentage)
                        {
                            numberOfAcceptableDecreasedRate++;
                            zonesWithDecreasedRatesAboveAcceptable.Add(string.Format("Rate Decrease on zone '{0}' from '{1}' to '{2}' has percentage higher than the specified acceptable decrease rate percentage '{3}%'", importedRate.ZoneName, recentRateFormatted, newRateFormatted, acceptableDecreaseRatePercentage));
                        }
                    }
                }
                if (zonesWithDecreasedRatesAboveAcceptable.Count()>0)
                contextMessages.Add(string.Format("{0} decreased Rate(s) above acceptable decrease rate percentage, following rate(s) are: {1}", numberOfAcceptableDecreasedRate, string.Join(", ", zonesWithDecreasedRatesAboveAcceptable)));
            }

            if (contextMessages.Any())
            {
                context.Message = string.Join(";", contextMessages);
                isValid = false;
            }
            return isValid;
        
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
