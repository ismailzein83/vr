using System;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using System.Linq;
using System.Collections.Generic;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class RateIncreasedCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is ImportedCountry;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var importedCountry = context.Target as ImportedCountry;

            if (importedCountry.ImportedRates == null || importedCountry.ImportedRates.Count == 0)
                return true;

            var invalidZoneNames = new HashSet<string>();
            IImportSPLContext importSplContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedRate importedRate in importedCountry.ImportedRates)
            {
                if (importedRate.ChangedExistingRates.Count() > 0 && importedRate.ChangedExistingRates.Any(x => importedRate.Rate > x.RateEntity.Rate))
                    invalidZoneNames.Add(importedRate.ZoneName);
            }

            if (invalidZoneNames.Count > 0)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(importedCountry.CountryId);
                context.Message = string.Format("Some of the rates of some of the zones of country '{0}' have been increased: {1}", countryName, string.Join(", ", invalidZoneNames));
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
