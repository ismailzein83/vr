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
            return target is CountryNotImportedCodes;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var countryNotImportedCodes = context.Target as CountryNotImportedCodes;

            if (countryNotImportedCodes.NotImportedCodes == null || countryNotImportedCodes.NotImportedCodes.Count() == 0)
                return true;

            var invalidCodes = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            DateTime codeClosureDate = importSPLContext.CodeEffectiveDate;

            foreach (NotImportedCode notImportedCode in countryNotImportedCodes.NotImportedCodes)
            {
                if (notImportedCode.HasChanged && notImportedCode.EED.HasValue && notImportedCode.EED.Value < codeClosureDate)
                    invalidCodes.Add(notImportedCode.ZoneName);
            }

            if (invalidCodes.Count > 0)
            {
                string codeClosureDateString = codeClosureDate.ToString(importSPLContext.DateFormat);
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryNotImportedCodes.CountryId);
                context.Message = string.Format("Closing code(s) of country '{0}' with a date less than '{1}'. Violated code(s): ({2}).", countryName, codeClosureDateString, string.Join(", ", invalidCodes));
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
