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
    public class CodeBEDGreaterThanActionDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is ImportedCountry;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            ImportedCountry importedCountry = context.Target as ImportedCountry;

            if (importedCountry.ImportedCodes == null || importedCountry.ImportedCodes.Count == 0)
                return true;

            var invalidCodes = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();

            foreach (ImportedCode importedCode in importedCountry.ImportedCodes)
            {
                if (importedCode.ChangeType == CodeChangeType.New || importedCode.ChangeType == CodeChangeType.Moved)
                {
                    if (importedCode.BED > importSPLContext.CodeEffectiveDate)
                        invalidCodes.Add(importedCode.Code);
                }
            }

            if (invalidCodes.Count > 0)
            {
                CountryManager countryManager = new CountryManager(); 
                string countryName = countryManager.GetCountryName(importedCountry.CountryId);
                string codeEffectiveDateString = importSPLContext.CodeEffectiveDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("Adding code(s) to country '{0}' with BED greater than '{1}'. Violated code(s): ({2}).", countryName, codeEffectiveDateString, string.Join(", ", invalidCodes));
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
