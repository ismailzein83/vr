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
            return target is ImportedCountry;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var importedCountry = context.Target as ImportedCountry;

            if (importedCountry.ImportedRates == null || importedCountry.ImportedRates.Count == 0)
                throw new NullReferenceException("importedCountry.ImportedRates");

            var configManager = new ConfigManager();
            int acceptableDecreaseRatePercentage = configManager.GetPurchaseAcceptableDecreasedRate();
            var zonesWithDecreasedRatesAboveAcceptable = new List<string>();
            int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
            var numberOfAcceptableDecreasedRate = 0;

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
                        zonesWithDecreasedRatesAboveAcceptable.Add(string.Format("Rate Decrease on zone '{0}' from '{1}' to '{2}' has percentage higher than the specified acceptable decrease rate percentage '{3}'", importedRate.ZoneName, recentRateFormatted, newRateFormatted, acceptableDecreaseRatePercentage));
                    }
                }
            }

            if (zonesWithDecreasedRatesAboveAcceptable.Any())
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(importedCountry.CountryId);
                context.Message = string.Format("{0} decreased Rate(s) above acceptable decrease rate percentage, following rate(s) are: {1}", numberOfAcceptableDecreasedRate, string.Join(", ", zonesWithDecreasedRatesAboveAcceptable));
                return false;
            }
            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
