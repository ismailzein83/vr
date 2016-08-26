using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MultipleCountryCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone zone = context.Target as ImportedDataByZone;

            bool result = true;

            if (zone.ImportedCodes != null)
            {
                var firstCode = zone.ImportedCodes.FirstOrDefault();
                if (firstCode != null)
                {
                    int? firstCodeCountryId = firstCode.CodeGroup != null ? firstCode.CodeGroup.CountryId : (int?)null;
                    Func<ImportedCode, bool> pred = new Func<ImportedCode, bool>((code) => code.CodeGroup != null && firstCodeCountryId.HasValue && code.CodeGroup.CountryId != firstCodeCountryId.Value);
                    result = !zone.ImportedCodes.Any(pred);
                }
            }

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has multiple codes that belong to different countries", (target as ImportedDataByZone).ZoneName);
        }

    }
}
