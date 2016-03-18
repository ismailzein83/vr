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
            return (target as ImportedZone != null);
        }

        public override bool Validate(IRuleTarget target)
        {
            ImportedZone zone = target as ImportedZone;

            var distinctImportedRates = from importedRate in zone.ImportedRates
                                    group importedRate by importedRate.NormalRate into newRates
                                    select newRates;

            return !(distinctImportedRates.Count() > 1);
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has different rate", (target as ImportedZone).ZoneName);
        }

    }
}
