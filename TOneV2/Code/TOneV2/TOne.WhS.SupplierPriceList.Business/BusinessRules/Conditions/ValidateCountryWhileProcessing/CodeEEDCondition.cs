using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeEEDCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllCountriesNotImportedCodes;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            DateTime codeClosureDate = importSPLContext.CodeEffectiveDate;
            bool isValid = true;
            var allCountriesNotImportedCodes = context.Target as AllCountriesNotImportedCodes;
            var contextMessages = new List<string>();
            var invalidCodesByCountryName = new List<string>();
            string codeClosureDateString = codeClosureDate.ToString(importSPLContext.DateFormat);

            foreach (var countryNotImportedCodes in allCountriesNotImportedCodes.CountriesNotImportedCodes)
            {
                if (countryNotImportedCodes.NotImportedCodes == null || countryNotImportedCodes.NotImportedCodes.Count() == 0)
                    continue;

                var invalidCodes = new HashSet<string>();

                foreach (NotImportedCode notImportedCode in countryNotImportedCodes.NotImportedCodes)
                {
                    if (notImportedCode.HasChanged && notImportedCode.EED.HasValue && notImportedCode.EED.Value < codeClosureDate)
                        invalidCodes.Add(notImportedCode.ZoneName);
                }
                if (invalidCodes.Count > 0)
                {
                    string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryNotImportedCodes.CountryId);
                    invalidCodesByCountryName.Add(string.Format("'{0} : {1}'",countryName, string.Join(",", invalidCodes)));
                }
            }
            if (invalidCodesByCountryName.Count > 0)
            {
                context.Message = string.Format("Closing code(s) with date less than '{0}'.Violated code(s) : '{1}'", codeClosureDateString, string.Join(" ; ", invalidCodesByCountryName));
                isValid =  false;
            }

            return isValid;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
