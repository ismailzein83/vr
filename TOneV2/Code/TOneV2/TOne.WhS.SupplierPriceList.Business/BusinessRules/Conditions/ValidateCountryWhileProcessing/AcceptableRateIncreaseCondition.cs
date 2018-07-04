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
    public class AcceptableRateIncreaseCondition : BusinessRuleCondition
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
            int acceptableIncreaseRatePercentage = configManager.GetPurchaseAcceptableIncreasedRate();
            var zonesWithIncreasedRatesAboveAcceptable = new List<string>();
            int longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
            var numberOfAcceptableincreasedRate = 0;

            foreach (ImportedRate importedRate in importedCountry.ImportedRates)
            {
                if (importedRate.ChangeType == RateChangeType.Increase)
                {
                    importedRate.ProcessInfo.RecentExistingRate.ThrowIfNull("RecentExistingRate");

                    var recentRate = importedRate.ProcessInfo.RecentExistingRate.ConvertedRate;
                    var newRate = importedRate.Rate;
                    var recentRateFormatted = Decimal.Round(recentRate, longPrecision);
                    var newRateFormatted = Decimal.Round(newRate, longPrecision);

                    var increasePercentage = ((1 - recentRate / newRate) * 100);

                    if (acceptableIncreaseRatePercentage < increasePercentage)
                    {
                        numberOfAcceptableincreasedRate++;
                        zonesWithIncreasedRatesAboveAcceptable.Add(string.Format("Rate increase on zone '{0}' from '{1}' to '{2}' at a percentage higher than the specified acceptable increase rate percentage '{3}'", importedRate.ZoneName, recentRateFormatted, newRateFormatted, acceptableIncreaseRatePercentage));
                    }
                }
            }

            if (zonesWithIncreasedRatesAboveAcceptable.Any())
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(importedCountry.CountryId);
                context.Message = string.Format("{0} rate(s) above acceptable increase rate percentage, following rate(s) are: {1}", numberOfAcceptableincreasedRate, string.Join(", ", zonesWithIncreasedRatesAboveAcceptable));
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
