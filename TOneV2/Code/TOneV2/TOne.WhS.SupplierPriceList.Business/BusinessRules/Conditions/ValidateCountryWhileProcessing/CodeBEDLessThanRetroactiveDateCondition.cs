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
    public class CodeBEDLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllImportedCountries;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            CountryManager countryManager = new CountryManager();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            bool isValid = true;
            var contextMessages = new List<string>();
            var allImportedCountries = context.Target as AllImportedCountries;
            string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);
            var invalidCodesByCountryname = new List<string>();

            foreach (var importedCountry in allImportedCountries.ImportedCountries)
            {
                var invalidCodes = new HashSet<string>();
            
                foreach (var importedCode in importedCountry.ImportedCodes)
                {
                    if (importedCode.BED < importSPLContext.RetroactiveDate && (importedCode.ChangeType == CodeChangeType.New || importedCode.ChangeType == CodeChangeType.Moved))
                    {
                        invalidCodes.Add(importedCode.Code);
                    }
                }
                if(invalidCodes.Count()>0)
                {
                    string countryName = countryManager.GetCountryName(importedCountry.CountryId);
                    invalidCodesByCountryname.Add(string.Format("'{0} : {1}'",countryName, string.Join(",", invalidCodes)));
                }
            }

            if (invalidCodesByCountryname.Count > 0)
            {
                context.Message = string.Format("Adding or moving codes with BED less than retroactive date '{0}'. Violated codes : '{1}'", retroactiveDateString, string.Join(" ; ", invalidCodesByCountryname));
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