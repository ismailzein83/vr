using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SameCodeWithDifferentZonesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCountry != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCountry country = context.Target as ImportedCountry;

            foreach (var importedCode in country.ImportedCodes)
            {
                if (country.ImportedCodes.Where(x => x.Code == importedCode.Code && !x.ZoneName.Equals(importedCode.ZoneName, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                {
                    CountryManager manager = new CountryManager();
                    context.Message = string.Format("Can not add Code {0} because Country {1} has this code in different zones", importedCode.Code, manager.GetCountryName(country.CountryId));
                    return false;
                }

            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            CountryManager manager = new CountryManager();
            return string.Format("Country {0} has same code in different zones", manager.GetCountryName((target as ImportedCountry).CountryId));
        }

    }
}
