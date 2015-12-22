using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MultipleCountryCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedZone != null);
        }

        public override bool Validate(IRuleTarget target)
        {
            ImportedZone zone = target as ImportedZone;

            if (zone == null)
                return false;

            bool result = true;

            if(zone.ImportedCodes != null)
            {
                var firstCode = zone.ImportedCodes.FirstOrDefault();
                if (firstCode != null)
                {
                    int? firstCodeCountryId = firstCode.CodeGroup != null ? firstCode.CodeGroup.CountryId : (int?)null;
                    Func<ImportedCode, bool> pred = new Func<ImportedCode, bool>((code) => code.CodeGroup != null && code.CodeGroup.CountryId != firstCodeCountryId.Value);
                    result = !zone.ImportedCodes.Any(pred);
                }
            }

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has multiple codes that belong to different countries", (target as ImportedZone).ZoneName);
        }

    }
}
