using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeBEDLessThanActionDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedCountries;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            bool isValid = true;
            CountryManager countryManager = new CountryManager();
            var allImportedCountries = context.Target as AllImportedCountries;
            var contextMessages = new List<string>();
            var invalidCodesByCountryName = new List<string>();
            string codeEffectiveDateString = importSPLContext.CodeEffectiveDate.ToString(importSPLContext.DateFormat);

            foreach (var importedCountry in allImportedCountries.ImportedCountries)
            {
                if (importedCountry.ImportedCodes == null || importedCountry.ImportedCodes.Count == 0)
                    continue;

                var invalidCodes = new HashSet<string>();
            

                foreach (ImportedCode importedCode in importedCountry.ImportedCodes)
                {
                    if (importedCode.ChangeType == CodeChangeType.New || importedCode.ChangeType == CodeChangeType.Moved)
                    {
                        if (importedCode.BED < importSPLContext.RetroactiveDate) // This condition is handled by CodeBEDLessThanRetroactiveDateCondition
                            continue;
                        if (importedCode.BED < importSPLContext.CodeEffectiveDate)
                            invalidCodes.Add(importedCode.Code);
                    }
                }
                if(invalidCodes.Count()>0)
                {
                    string countryName = countryManager.GetCountryName(importedCountry.CountryId);
                   
                    invalidCodesByCountryName.Add(string.Format("'{0} : {1}' ", countryName, string.Join(", ", invalidCodes)));
                }
            }

            if (invalidCodesByCountryName.Count > 0)
            {
                context.Message = string.Format("Adding code(s) to country(ies) with BED less than '{0}'.Violated country(ies):'{1}'", codeEffectiveDateString, string.Join(" ; ", invalidCodesByCountryName));
                isValid = false;
            }

            return isValid;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
