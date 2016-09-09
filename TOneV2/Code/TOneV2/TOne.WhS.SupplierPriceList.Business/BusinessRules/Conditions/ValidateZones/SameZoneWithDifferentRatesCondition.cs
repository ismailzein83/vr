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
    public class SameZoneWithDifferentRatesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone zone = context.Target as ImportedDataByZone;

            var distinctImportedNormalRates = from importedRate in zone.ImportedNormalRates
                                        where !importedRate.RateTypeId.HasValue
                                        group importedRate by importedRate.Rate into newRates
                                        select newRates;


            var distinctImportedOtherRates = from importedRate in zone.ImportedOtherRates
                                        where importedRate.RateTypeId.HasValue
                                        group importedRate by importedRate.RateTypeId into newRates
                                        select newRates;

            return !(distinctImportedNormalRates.Count() > 1 || distinctImportedOtherRates.Any(item => item.Count() > 1));
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has different rates", (target as ImportedDataByZone).ZoneName);
        }

    }
}
