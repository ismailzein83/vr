using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class OtherRateBEDLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedCountries;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            CountryManager countryManager = new CountryManager();
            bool isValid = true;
            var allImportedCountries = context.Target as AllImportedCountries;
            var contextMessages = new List<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
            var invalidZonesByCountryName = new List<string>();

            foreach (var importedCountry in allImportedCountries.ImportedCountries)
            {
                var invalidRateTypes = new HashSet<string>();
                var invalidZoneNames = new HashSet<string>();
                var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
          

                foreach (var importedZone in importedCountry.ImportedZones)
                {
                    foreach (var importedRate in importedZone.ImportedOtherRates.Values)
                    {
                        if (importedRate.RateTypeId.HasValue && importedRate.BED < importSPLContext.RetroactiveDate && (importedRate.ChangeType == RateChangeType.New || importedRate.ChangeType == RateChangeType.Decrease || importedRate.ChangeType == RateChangeType.Increase))
                        {
                            invalidZoneNames.Add(importedRate.ZoneName);
                            invalidRateTypes.Add(rateTypeManager.GetRateTypeName(importedRate.RateTypeId.Value));
                        }
                    }
                }
                if (invalidZoneNames.Count > 0)
                {
                    string countryName = countryManager.GetCountryName(importedCountry.CountryId);
                    invalidZonesByCountryName.Add(string.Format("'{0} : {1} ({2})'", countryName, string.Join(",", invalidZoneNames),string.Join(", ", invalidRateTypes)));
                }
            }
            if (invalidZonesByCountryName.Count > 0)
            {
                context.Message = string.Format("Changing rate(s) with date less than retroactive date '{0}' for the following zone(s) :'{1}'",retroactiveDateString,string.Join(" ; ",invalidZonesByCountryName));
               isValid  = false;
            }

            return isValid;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
