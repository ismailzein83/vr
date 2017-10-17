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

            var invalidZoneNames = new HashSet<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            DateTime codeClosureDate = DateTime.Today.AddDays(importSPLContext.CodeCloseDateOffset.Days);

            foreach (NotImportedCode notImportedCode in countryNotImportedCodes.NotImportedCodes)
            {
                if (notImportedCode.HasChanged && notImportedCode.EED.HasValue && notImportedCode.EED.Value < codeClosureDate)
                    invalidZoneNames.Add(notImportedCode.ZoneName);
            }

            if (invalidZoneNames.Count > 0)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryNotImportedCodes.CountryId);
                string codeClosureDateString = codeClosureDate.ToString(importSPLContext.DateFormat);
                context.Message = string.Format("EEDs of some of the codes of the following zones of country '{0}' are less than '{1}': {2}", countryName, codeClosureDateString, string.Join(", ", invalidZoneNames));
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
