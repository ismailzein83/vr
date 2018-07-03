using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

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
            //var importedCountry = context.Target as ImportedCountry;

            //if (importedCountry.ImportedRates == null || importedCountry.ImportedRates.Count == 0)
            //    return true;

            //var numberOfIncreasedRates = 0;
            //List<string> zonesWithIncreasedRates = new List<string>();
            //var longPrecision = new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
            //IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            //string codeEffectiveDateString = importSPLContext.CodeEffectiveDate.ToString(importSPLContext.DateFormat);
            //foreach (ImportedRate importedRate in importedCountry.ImportedRates)
            //{
            //    if (importedRate.ChangeType == RateChangeType.Increase)
            //    {
            //        numberOfIncreasedRates++;
            //        if (importedRate.BED < importSPLContext.CodeEffectiveDate)
            //        {
            //            importedRate.ProcessInfo.RecentExistingRate.ThrowIfNull("RecentExistingRate");
            //            var recentRate = Decimal.Round(importedRate.ProcessInfo.RecentExistingRate.ConvertedRate, longPrecision);
            //            var currentRate = Decimal.Round(importedRate.Rate, longPrecision);
            //            zonesWithIncreasedRates.Add(string.Format("Rate increase on zone '{0}' from '{1}' to '{2}' effective on '{3}' should be on '{4}'", importedRate.ZoneName, recentRate, currentRate, importedRate.BED.ToString(importSPLContext.DateFormat), codeEffectiveDateString));
            //        }
            //    }
            //}

            //if (numberOfIncreasedRates > 0)
            //{
            //    string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(importedCountry.CountryId);
            //    context.Message = string.Format("{0} increased rate(s) in country '{1}'. {2}", numberOfIncreasedRates, countryName, string.Join(", ", zonesWithIncreasedRates));
            //    return false;
            //}
            return true;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
