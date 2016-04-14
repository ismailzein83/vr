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

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("Target");

            ImportedZone zone = context.Target as ImportedZone;
            if (zone.ImportedRates != null)
                return !(zone.ImportedRates.Count() == 0);

            return false;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a missing rate",(target as ImportedZone).ZoneName);
        }

    }
}
