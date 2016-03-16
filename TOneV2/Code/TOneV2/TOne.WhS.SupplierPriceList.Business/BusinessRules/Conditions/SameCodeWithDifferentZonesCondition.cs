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

        public override bool Validate(IRuleTarget target)
        {
            ImportedCountry country = target as ImportedCountry;


            foreach (var importedCode in country.ImportedCodes)
            {
                if (country.ImportedCodes.Where(x => x.Code == importedCode.Code && x.ZoneName != importedCode.ZoneName).Count() > 0)
                    return false;
            }


            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            CountryManager manager = new CountryManager();
            return string.Format("Country {0} has one or more same Code on Different Zones", manager.GetCountryName((target as ImportedCountry).CountryId));
        }

    }
}
