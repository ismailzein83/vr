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
    public class MissingRatesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedZone != null);
        }

        public override bool Validate(IRuleTarget target)
        {
            ImportedZone zone = target as ImportedZone;

            foreach (var importedRate in zone.ImportedRates)
            {
                if (string.IsNullOrEmpty(importedRate.NormalRate.ToString()))
                    return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("The Zone {0}  has a Missing Rate",target.Key);
        }

    }
}
