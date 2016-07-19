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
    public class MissingCodesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
           
            ImportedZone zone = context.Target as ImportedZone;

            foreach (var importedCode in zone.ImportedCodes)
            {
                if (string.IsNullOrEmpty(importedCode.Code))
                    return false;
            }

            return true;            
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a missing code",(target as ImportedZone).ZoneName);
        }

    }
}
